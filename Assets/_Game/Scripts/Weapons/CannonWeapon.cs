using UnityEngine;

namespace IronTide.Weapons
{
    /// <summary>
    /// Fires arcing cannon shells from an object pool.
    /// Reads damage bonuses from ShipStats (upgrade system).
    /// </summary>
    public class CannonWeapon : WeaponBase
    {
        [SerializeField] private Utils.ObjectPool shellPool;

        protected override void SpawnProjectile(Vector3 adjustedTarget)
        {
            if (shellPool == null)
            {
                Debug.LogWarning("[CannonWeapon] No shell pool assigned.");
                return;
            }

            // Compute damage with upgrade bonuses
            float baseDmg = weaponData.baseDamage;
            float variance = baseDmg * weaponData.damageVariance;
            float damage   = baseDmg + Random.Range(-variance, variance);
            if (Stats != null) damage += Stats.CannonDamageBonus;

            var go    = shellPool.Get(muzzlePoint.position, Quaternion.identity);
            var shell = go.GetComponent<CannonShell>();
            if (shell == null)
            {
                Debug.LogError("[CannonWeapon] Pool prefab missing CannonShell component.");
                shellPool.Return(go);
                return;
            }

            shell.InitShell(weaponData, OwnerId, OwnerTeam, damage,
                            muzzlePoint.position, adjustedTarget,
                            shellPool.gameObject);

            // Muzzle flash VFX
            if (weaponData.muzzleFlashPrefab != null)
                Instantiate(weaponData.muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);

            // SFX
            if (weaponData.fireSound != null)
                AudioSource.PlayClipAtPoint(weaponData.fireSound, muzzlePoint.position);
        }
    }
}
