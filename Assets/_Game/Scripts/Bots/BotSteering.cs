using UnityEngine;
using IronTide.Ships;

namespace IronTide.Bots
{
    /// <summary>
    /// Simple waypoint-based steering for bots on the water plane.
    /// Uses ShipController's movement system (sets a synthetic MoveInput via BotInputAdapter).
    /// Phase 2: Replace with NavMesh agent if terrain is complex.
    /// </summary>
    [RequireComponent(typeof(ShipController))]
    public class BotSteering : MonoBehaviour
    {
        [SerializeField] private float waypointRadius = 2f;
        [SerializeField] private float avoidanceRadius = 5f;

        private Vector3?       _destination;
        private ShipController _controller;

        // BotSteering drives the ship by directly setting velocity via the controller's internal method.
        // In Phase 2 this will use a proper bot input adapter rather than reflection.

        private void Awake() => _controller = GetComponent<ShipController>();

        public void MoveTo(Vector3 destination) => _destination = destination;

        public void Stop() => _destination = null;

        private void Update()
        {
            if (_destination == null) return;

            Vector3 toTarget = _destination.Value - transform.position;
            toTarget.y = 0f;

            if (toTarget.magnitude < waypointRadius)
            {
                _destination = null;
                return;
            }

            // The bot sets its intended forward direction; ShipController applies speed.
            // This is a stub — Phase 2 will integrate with a proper BotInputAdapter.
            float angle = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.Euler(0f, angle, 0f),
                GetComponent<ShipStats>().TurnRate * Time.deltaTime);
        }
    }
}
