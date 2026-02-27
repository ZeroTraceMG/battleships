using UnityEngine;

namespace IronTide.Weapons
{
    /// <summary>
    /// Parabolic arc cannon shell. Moves from origin to target over flightTime,
    /// then AoE splash damages all ShipHealth components within blastRadius.
    /// </summary>
    public class CannonShell : ProjectileBase
    {
        private Vector3 _origin;
        private Vector3 _targetPos;
        private float   _flightTime;
        private float   _elapsed;
        private bool    _landed;

        // ── Re-init on pool retrieve ────────────────────────────────────────

        public void InitShell(WeaponData data, string attackerId, int attackerTeam,
                               float damage, Vector3 origin, Vector3 targetPos,
                               GameObject poolRef)
        {
            base.Init(data, attackerId, attackerTeam, damage, poolRef);
            _origin     = origin;
            _targetPos  = targetPos;
            _flightTime = Vector3.Distance(origin, targetPos) / data.projectileSpeed;
            _elapsed    = 0f;
            _landed     = false;

            transform.position = origin;
        }

        protected override void Update()
        {
            if (_landed) return;

            _elapsed += Time.deltaTime;
            float t  = Mathf.Clamp01(_elapsed / _flightTime);

            // Parabolic arc: linear XZ interpolation + sine Y arc
            var pos     = Vector3.Lerp(_origin, _targetPos, t);
            pos.y       += Data.arcHeight * Mathf.Sin(t * Mathf.PI);
            transform.position = pos;

            // Face movement direction
            if (t < 1f)
            {
                var nextPos = Vector3.Lerp(_origin, _targetPos, t + 0.01f);
                nextPos.y   += Data.arcHeight * Mathf.Sin((t + 0.01f) * Mathf.PI);
                var dir     = (nextPos - pos).normalized;
                if (dir.sqrMagnitude > 0f)
                    transform.rotation = Quaternion.LookRotation(dir);
            }

            if (t >= 1f)
                Land();
        }

        private void Land()
        {
            _landed = true;

            // Splash AoE
            var hits = Physics.OverlapSphere(_targetPos, Data.blastRadius,
                           Utils.LayerMaskConstants.HittableMask);
            foreach (var col in hits)
            {
                var health = col.GetComponent<Ships.ShipHealth>();
                if (health != null)
                    OnHit(col);
            }

            // VFX placeholder (swap for particle effect in Phase 2)
            // Instantiate(explosionPrefab, _targetPos, Quaternion.identity);

            ReturnToPool();
        }

        protected override void OnHit(Collider other)
        {
            var health = other.GetComponent<Ships.ShipHealth>();
            health?.TakeDamage(Damage, AttackerId);
        }
    }
}
