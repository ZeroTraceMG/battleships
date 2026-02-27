using UnityEngine;

namespace IronTide.Weapons
{
    public enum WeaponType  { Cannon, Torpedo, Utility }
    public enum DamageType  { Kinetic, Explosive, Fire }

    [CreateAssetMenu(fileName = "WD_", menuName = "IronTide/Weapon")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string     weaponId;
        public string     displayName;
        public WeaponType weaponType;

        [Header("Projectile")]
        public GameObject projectilePrefab;
        public float      projectileSpeed    = 20f;
        public float      projectileLifetime = 4f;
        public float      arcHeight          = 3f;   // parabolic peak (Cannon)
        public float      blastRadius        = 2.5f; // AoE splash radius
        public float      steeringFactor     = 0f;   // >0 = torpedo drift

        [Header("Damage")]
        public float      baseDamage         = 80f;
        [Range(0f, 0.5f)]
        public float      damageVariance     = 0.1f; // ±10% by default
        public DamageType damageType         = DamageType.Explosive;

        [Header("Firing")]
        public float      cooldown           = 3f;
        public int        salvoCount         = 1;
        public float      salvoInterval      = 0.2f;
        [Range(0f, 30f)]
        public float      spread             = 0f;   // degrees per salvo shell

        [Header("Range")]
        public float      minRange           = 2f;
        public float      maxRange           = 30f;

        [Header("Audio / VFX")]
        public AudioClip  fireSound;
        public GameObject muzzleFlashPrefab;
    }
}
