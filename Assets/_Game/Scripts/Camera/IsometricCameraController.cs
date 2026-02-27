using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace IronTide.Camera
{
    /// <summary>
    /// Isometric camera: follows a target with zoom (scroll / pinch) and
    /// rotation (middle-mouse drag / two-finger swipe).
    /// Attach to the Cinemachine VirtualCamera's parent or the camera rig root.
    /// </summary>
    public class IsometricCameraController : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private Transform target;
        [SerializeField] private float     followSmoothing = 8f;

        [Header("Zoom")]
        [SerializeField] private float zoomMin     = 3f;
        [SerializeField] private float zoomMax     = 40f;
        [SerializeField] private float zoomCurrent = 15f;
        [SerializeField] private float zoomSpeed   = 5f;
        [SerializeField] private float zoomSmooth  = 10f;

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed    = 120f;   // deg/sec for keyboard/drag
        [SerializeField] private float rotationSmoothing = 8f;
        private float _targetYaw;
        private float _currentYaw;

        [Header("Pitch (fixed isometric angle)")]
        [SerializeField] private float pitch = 45f;

        // Internal state
        private float _targetZoom;
        private Vector3 _followPos;
        private bool  _middleDragActive;
        private Vector2 _lastMiddleDragPos;
        private float _lastPinchDist;

        private void Awake()
        {
            EnhancedTouchSupport.Enable();
            _targetZoom  = zoomCurrent;
            _targetYaw   = transform.eulerAngles.y;
            _currentYaw  = _targetYaw;

            if (target == null)
                Debug.LogWarning("[IsometricCameraController] No follow target assigned.");
        }

        private void LateUpdate()
        {
            if (target != null)
                _followPos = Vector3.Lerp(_followPos, target.position, Time.deltaTime * followSmoothing);

            HandlePCInput();
            HandleTouchInput();

            // Smooth zoom
            zoomCurrent = Mathf.Lerp(zoomCurrent, _targetZoom, Time.deltaTime * zoomSmooth);
            _currentYaw  = Mathf.LerpAngle(_currentYaw, _targetYaw, Time.deltaTime * rotationSmoothing);

            // Apply position and rotation
            var rotation = Quaternion.Euler(pitch, _currentYaw, 0f);
            transform.rotation = rotation;
            transform.position = _followPos - rotation * Vector3.forward * zoomCurrent;
        }

        private void HandlePCInput()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;

            // Scroll zoom
            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
                _targetZoom = Mathf.Clamp(_targetZoom - scroll * zoomSpeed * Time.deltaTime * 60f, zoomMin, zoomMax);

            // Middle-mouse drag to rotate
            if (mouse.middleButton.wasPressedThisFrame)
            {
                _middleDragActive  = true;
                _lastMiddleDragPos = mouse.position.ReadValue();
            }
            if (mouse.middleButton.wasReleasedThisFrame)
                _middleDragActive = false;

            if (_middleDragActive)
            {
                Vector2 currentPos = mouse.position.ReadValue();
                float   dx         = currentPos.x - _lastMiddleDragPos.x;
                _targetYaw        += dx * rotationSpeed * Time.deltaTime;
                _lastMiddleDragPos = currentPos;
            }
        }

        private void HandleTouchInput()
        {
            var activeTouches = Touch.activeTouches;
            if (activeTouches.Count < 2) { _lastPinchDist = 0f; return; }

            var t0 = activeTouches[0];
            var t1 = activeTouches[1];

            float pinchDist = Vector2.Distance(t0.screenPosition, t1.screenPosition);

            if (_lastPinchDist > 0f)
            {
                float delta     = pinchDist - _lastPinchDist;
                _targetZoom     = Mathf.Clamp(_targetZoom - delta * 0.05f, zoomMin, zoomMax);

                // Two-finger swipe: average horizontal delta → rotation
                float avgDeltaX = (t0.delta.x + t1.delta.x) * 0.5f;
                _targetYaw     += avgDeltaX * rotationSpeed * Time.deltaTime * 0.5f;
            }

            _lastPinchDist = pinchDist;
        }

        /// <summary>Set a new follow target at runtime (called when ship spawns).</summary>
        public void SetTarget(Transform newTarget) => target = newTarget;
    }
}
