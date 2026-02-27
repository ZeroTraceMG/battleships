using UnityEngine;

namespace IronTide.Economy
{
    public enum UpgradeCategory { Hull, Engine, Cannon, Torpedo, Utility }

    [System.Serializable]
    public struct StatModifier
    {
        public StatType statType;
        public ModifierMode mode;   // Additive or Multiplicative
        public float value;         // additive: +50 HP | multiplicative: 1.08 = +8%
    }

    public enum StatType
    {
        MaxHP, Armor, Speed, TurnRate, Acceleration,
        CannonDamage, CannonRange, CannonCooldown,
        TorpedoDamage, TorpedoSpeed,
        AbilityCooldown, RepairAmount
    }

    public enum ModifierMode { Additive, Multiplicative }

    [CreateAssetMenu(fileName = "UP_", menuName = "IronTide/Upgrade")]
    public class UpgradeData : ScriptableObject
    {
        [Header("Identity")]
        public string          upgradeId;
        public UpgradeCategory category;
        [Range(1, 5)]
        public int             level;
        public string          displayName;
        public Sprite          icon;

        [Header("Cost")]
        public int goldCost;

        [Header("Bonuses")]
        public StatModifier[]  bonuses;

        [Header("Prerequisite")]
        public UpgradeData     prerequisite; // null = no prereq (level I)
    }
}
