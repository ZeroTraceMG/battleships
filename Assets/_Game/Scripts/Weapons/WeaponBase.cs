using System.Collections;
using UnityEngine;

namespace IronTide.Weapons
{
    /// <summary>
    /// Abstract base for all ship weapons. Handles cooldown and salvo timing.
    /// Subclasses implement SpawnProjectile() to create their specific projectile type.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData weaponData;
        [SerializeField] protected Transform  muzzlePoint;

        protected Ships.ShipStats  Stats;
        protected string           OwnerId;
        protected int              OwnerTeam;

        private float _cooldownRemaining;
        private bool  _isFiring;

        protected virtual void Awake()
        {
            Stats = GetComponentInParent<Ships.ShipStats>();
            if (Stats == null)
                Debug.LogError($"[{GetType().Name}] No ShipStats found in parent hierarchy.");
        }

        public void Init(string ownerId, int ownerTeam)
        {
            OwnerId   = ownerId;
            OwnerTeam = ownerTeam;
        }

        protected virtual void Update()
        {
            if (_cooldownRemaining > 0f)
                _cooldownRemaining -= Time.deltaTime;
        }

        public bool CanFire => _cooldownRemaining <= 0f && !_isFiring && weaponData != null;

        /// <summary>Attempt to fire. Returns false if on cooldown.</summary>
        public bool TryFire(Vector3 targetPosition)
        {
            if (!CanFire) return false;

            float effectiveCooldown = weaponData.cooldown;
            if (Stats != null) effectiveCooldown *= Stats.CannonCooldownMult;

            _cooldownRemaining = effectiveCooldown;
            StartCoroutine(SalvoCoroutine(targetPosition));
            return true;
        }

        private IEnumerator SalvoCoroutine(Vector3 targetPosition)
        {
            _isFiring = true;
            for (int i = 0; i < weaponData.salvoCount; i++)
            {
                float spreadAngle = (i - (weaponData.salvoCount - 1) * 0.5f) * weaponData.spread;
                var   rotated     = Quaternion.AngleAxis(spreadAngle, Vector3.up)
                                  * (targetPosition - muzzlePoint.position).normalized;
                var   adjustedTarget = muzzlePoint.position + rotated * weaponData.maxRange;

                SpawnProjectile(adjustedTarget);

                if (weaponData.salvoCount > 1 && i < weaponData.salvoCount - 1)
                    yield return new WaitForSeconds(weaponData.salvoInterval);
            }
            _isFiring = false;
        }

        /// <summary>Create and launch one projectile toward adjustedTarget.</summary>
        protected abstract void SpawnProjectile(Vector3 adjustedTarget);

        public WeaponData Data => weaponData;
    }
}
