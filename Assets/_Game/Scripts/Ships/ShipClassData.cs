using UnityEngine;
using IronTide.Weapons;

namespace IronTide.Ships
{
    public enum ShipArchetype { Corvette, Frigate, Destroyer, Ironclad }

    [CreateAssetMenu(fileName = "SC_", menuName = "IronTide/Ship Class")]
    public class ShipClassData : ScriptableObject
    {
        [Header("Identity")]
        public string       shipId;
        public string       displayName;
        public Sprite       icon;
        public GameObject   shipPrefab;
        public ShipArchetype archetype;

        [Header("Base Stats")]
        public float baseMaxHP          = 1000f;
        public float baseArmor          = 0f;      // flat damage reduction %
        public float baseSpeed          = 8f;      // units/sec
        public float baseTurnRate       = 90f;     // deg/sec
        public float baseAcceleration   = 12f;

        [Header("Weapons")]
        public WeaponData primaryWeapon;
        public WeaponData secondaryWeapon;

        [Header("Economy")]
        public int purchaseCost = 0;   // 0 = free starting ship
        public int tier         = 1;   // 1 Corvette → 4 Ironclad

        [Header("Upgrade Caps (max level per category)")]
        public int maxHullLevel     = 5;
        public int maxEngineLevel   = 5;
        public int maxCannonLevel   = 5;
        public int maxTorpedoLevel  = 5;
        public int maxUtilityLevel  = 5;

        [Header("Respawn")]
        public float respawnTimeBase   = 8f;    // seconds
        public float respawnTimePerTier = 2f;   // added per tier above 1
    }
}
