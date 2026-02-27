using UnityEngine;

namespace IronTide.Weapons
{
    /// <summary>Fires torpedo projectiles in the aimed direction.</summary>
    public class TorpedoWeapon : WeaponBase
    {
        [SerializeField] private Utils.ObjectPool torpedoPool;

        protected override void SpawnProjectile(Vector3 adjustedTarget)
        {
            if (torpedoPool == null) { Debug.LogWarning("[TorpedoWeapon] No pool assigned."); return; }

            float damage   = weaponData.baseDamage;
            float variance = damage * weaponData.damageVariance;
            damage         = damage + Random.Range(-variance, variance);
            if (Stats != null) damage += Stats.TorpedoDamageBonus;

            Vector3 dir = (adjustedTarget - muzzlePoint.position);
            dir.y       = 0f;
            dir         = dir.normalized;

            var go         = torpedoPool.Get(muzzlePoint.position, Quaternion.LookRotation(dir));
            var torpedo    = go.GetComponent<TorpedoProjectile>();
            if (torpedo == null) { torpedoPool.Return(go); return; }

            torpedo.InitTorpedo(weaponData, OwnerId, OwnerTeam, damage, dir, torpedoPool.gameObject);

            if (weaponData.fireSound != null)
                AudioSource.PlayClipAtPoint(weaponData.fireSound, muzzlePoint.position);
        }
    }
}
