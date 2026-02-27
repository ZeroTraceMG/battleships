using UnityEngine;

namespace IronTide.Weapons
{
    /// <summary>
    /// Skillshot torpedo: moves linearly with optional passive drift (steeringFactor).
    /// No lock-on. Explodes on contact with a ship or map object.
    /// </summary>
    public class TorpedoProjectile : ProjectileBase
    {
        private Vector3 _direction;
        private float   _speed;

        public void InitTorpedo(WeaponData data, string attackerId, int attackerTeam,
                                 float damage, Vector3 direction, GameObject poolRef)
        {
            base.Init(data, attackerId, attackerTeam, damage, poolRef);
            _direction = direction.normalized;
            _speed     = data.projectileSpeed;
        }

        protected override void Update()
        {
            base.Update(); // handles lifetime

            // Linear movement with mild drift
            if (Data != null && Data.steeringFactor > 0f)
            {
                // Slight curve: rotate direction very slowly (cosmetic drift)
                _direction = Quaternion.AngleAxis(Data.steeringFactor * Time.deltaTime,
                             Vector3.up) * _direction;
            }

            transform.position += _direction * (_speed * Time.deltaTime);
            if (_direction.sqrMagnitude > 0f)
                transform.rotation = Quaternion.LookRotation(_direction);
        }

        protected override void OnHit(Collider other)
        {
            var health = other.GetComponent<Ships.ShipHealth>();
            health?.TakeDamage(Damage, AttackerId);
            ReturnToPool();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // Also hit Harbor/objective
            var health  = other.GetComponent<Ships.ShipHealth>();
            var harbor  = other.GetComponent<Map.Harbor>();

            if (health != null || harbor != null)
                OnHit(other);
        }
    }
}
