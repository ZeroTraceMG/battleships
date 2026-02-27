using UnityEngine;
using IronTide.Core;
using IronTide.Input;

namespace IronTide.Ships
{
    /// <summary>
    /// Moves the ship based on IInputHandler. Uses ShipStats for speed/turn caps.
    /// Kinematic Rigidbody: no gravity, Y position locked.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(ShipStats))]
    public class ShipController : MonoBehaviour
    {
        [Header("Mouse aim plane height")]
        [SerializeField] private float waterY = 0f;

        private Rigidbody   _rb;
        private ShipStats   _stats;
        private InputRouter _inputRouter;

        private Vector3 _velocity;
        private float   _currentYaw;
        private Vector3 _aimWorldPos;   // world point ship aims toward

        private void Awake()
        {
            _rb    = GetComponent<Rigidbody>();
            _stats = GetComponent<ShipStats>();

            _rb.isKinematic   = true;
            _rb.useGravity    = false;
            _rb.constraints   = RigidbodyConstraints.FreezePositionY
                              | RigidbodyConstraints.FreezeRotationX
                              | RigidbodyConstraints.FreezeRotationZ;

            _currentYaw  = transform.eulerAngles.y;
        }

        private void Start()
        {
            // Retrieve InputRouter once scene is ready
            _inputRouter = ServiceLocator.Get<InputRouter>();
        }

        private void Update()
        {
            if (_inputRouter == null) return;

            var handler = _inputRouter.ActiveHandler;
            UpdateRotation(handler);
            UpdateVelocity(handler);
        }

        private void FixedUpdate()
        {
            var newPos = _rb.position + _velocity * Time.fixedDeltaTime;
            newPos.y   = waterY;
            _rb.MovePosition(newPos);
            _rb.MoveRotation(Quaternion.Euler(0f, _currentYaw, 0f));
        }

        private void UpdateRotation(IInputHandler handler)
        {
            // Determine desired heading from aim input
            Vector3 aimDir = Vector3.zero;

            if (handler.AimDirection != Vector3.zero)
            {
                // PCInputHandler stores world position in AimDirection.xyz
                // MobileInputHandler stores direction directly
                var aimed = handler.AimDirection;
                if (aimed.sqrMagnitude > 1.1f) // world pos (PC): compute direction
                    aimDir = (new Vector3(aimed.x, waterY, aimed.z) - transform.position).normalized;
                else
                    aimDir = new Vector3(aimed.x, 0f, aimed.z).normalized;
            }
            else if (handler.MoveInput.sqrMagnitude > 0.01f)
            {
                // Fallback: face movement direction
                aimDir = new Vector3(handler.MoveInput.x, 0f, handler.MoveInput.y).normalized;
            }

            if (aimDir.sqrMagnitude > 0.01f)
            {
                float targetYaw  = Mathf.Atan2(aimDir.x, aimDir.z) * Mathf.Rad2Deg;
                float maxDelta   = _stats.TurnRate * Time.deltaTime;
                _currentYaw      = Mathf.MoveTowardsAngle(_currentYaw, targetYaw, maxDelta);
            }
        }

        private void UpdateVelocity(IInputHandler handler)
        {
            Vector2 moveInput = handler.MoveInput;
            if (moveInput.sqrMagnitude < 0.01f)
            {
                // Decelerate
                _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, _stats.Acceleration * Time.deltaTime);
                return;
            }

            // Forward is ship's facing; strafe disabled for naval feel
            Vector3 forward  = new Vector3(Mathf.Sin(_currentYaw * Mathf.Deg2Rad), 0f,
                                           Mathf.Cos(_currentYaw * Mathf.Deg2Rad));
            Vector3 desired  = forward * moveInput.y * _stats.Speed;

            _velocity = Vector3.MoveTowards(_velocity, desired, _stats.Acceleration * Time.deltaTime);

            // Server-side capped in networked mode; replicate cap here for offline
            if (_velocity.magnitude > _stats.Speed)
                _velocity = _velocity.normalized * _stats.Speed;
        }

        /// <summary>Current world-space velocity — exposed for network sync.</summary>
        public Vector3 Velocity => _velocity;
    }
}
