using UnityEngine;
using IronTide.Weapons;

namespace IronTide.Bots
{
    /// <summary>
    /// Scans for enemies, leads the shot based on BotTuningData.leadShotFactor,
    /// and triggers weapon fire.
    /// </summary>
    public class BotCombatModule : MonoBehaviour
    {
        [SerializeField] private BotTuningData tuningData;
        [SerializeField] private WeaponBase    primaryWeapon;
        [SerializeField] private float         scanRadius = 25f;

        private bool   _attacking;
        private float  _reactionTimer;

        private void Awake()
        {
            if (primaryWeapon == null)
                primaryWeapon = GetComponentInChildren<WeaponBase>();
        }

        public void TryAttack()
        {
            _attacking = true;
        }

        public void StopAttacking() => _attacking = false;

        private void Update()
        {
            if (!_attacking) return;

            _reactionTimer -= Time.deltaTime;
            if (_reactionTimer > 0f) return;

            var target = FindNearestEnemy();
            if (target == null) return;

            _reactionTimer = tuningData != null ? tuningData.reactionTime : 0.5f;

            Vector3 aimPos = LeadTarget(target);

            // Apply aim error
            float err = tuningData != null ? tuningData.aimErrorDegrees : 5f;
            aimPos   += Random.insideUnitSphere * (err * Mathf.Deg2Rad * scanRadius);
            aimPos.y  = 0f;

            primaryWeapon?.TryFire(aimPos);
        }

        private Transform FindNearestEnemy()
        {
            var myTeam   = GetComponent<Ships.ShipTeam>();
            int myTeamId = myTeam != null ? myTeam.TeamId : -1;

            var cols = Physics.OverlapSphere(transform.position, scanRadius,
                           Utils.LayerMaskConstants.ShipsMask);

            Transform best = null;
            float     bestDist = float.MaxValue;

            foreach (var col in cols)
            {
                var team = col.GetComponent<Ships.ShipTeam>();
                if (team == null || team.TeamId == myTeamId) continue;

                float d = Vector3.Distance(transform.position, col.transform.position);
                if (d < bestDist) { bestDist = d; best = col.transform; }
            }
            return best;
        }

        private Vector3 LeadTarget(Transform target)
        {
            if (tuningData == null || primaryWeapon == null)
                return target.position;

            float lead      = tuningData.leadShotFactor;
            float shellSpeed = primaryWeapon.Data?.projectileSpeed ?? 20f;
            float dist      = Vector3.Distance(transform.position, target.position);
            float timeToHit = dist / shellSpeed;

            var rb = target.GetComponent<Rigidbody>();
            if (rb == null) return target.position;

            return Vector3.Lerp(target.position, target.position + rb.linearVelocity * timeToHit, lead);
        }
    }
}
