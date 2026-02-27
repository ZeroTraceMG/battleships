using System.Collections.Generic;
using UnityEngine;
using IronTide.Economy;

namespace IronTide.Ships
{
    /// <summary>
    /// Runtime stat container for a ship. Populated from ShipClassData on spawn,
    /// then modified whenever the player purchases upgrades.
    /// Call Recalculate() after any upgrade is applied.
    /// </summary>
    public class ShipStats : MonoBehaviour
    {
        // --- Base (from ShipClassData, set once) ---
        public float BaseMaxHP          { get; private set; }
        public float BaseArmor          { get; private set; }
        public float BaseSpeed          { get; private set; }
        public float BaseTurnRate       { get; private set; }
        public float BaseAcceleration   { get; private set; }

        // --- Effective (base + upgrades applied) ---
        public float MaxHP        { get; private set; }
        public float Armor        { get; private set; }   // flat damage reduction %
        public float Speed        { get; private set; }
        public float TurnRate     { get; private set; }
        public float Acceleration { get; private set; }

        // Weapon stat modifiers (applied by UpgradeData)
        public float CannonDamageBonus    { get; private set; }
        public float CannonRangeBonus     { get; private set; }
        public float CannonCooldownMult   { get; private set; } = 1f;
        public float TorpedoDamageBonus   { get; private set; }
        public float TorpedoSpeedBonus    { get; private set; }
        public float AbilityCooldownMult  { get; private set; } = 1f;
        public float RepairAmountBonus    { get; private set; }

        private readonly List<UpgradeData> _upgrades = new();

        public void InitFromData(ShipClassData data)
        {
            BaseMaxHP        = data.baseMaxHP;
            BaseArmor        = data.baseArmor;
            BaseSpeed        = data.baseSpeed;
            BaseTurnRate     = data.baseTurnRate;
            BaseAcceleration = data.baseAcceleration;
            Recalculate();
        }

        public void AddUpgrade(UpgradeData upgrade)
        {
            _upgrades.Add(upgrade);
            Recalculate();
        }

        /// <summary>Recompute all effective stats from base + all purchased upgrades.</summary>
        public void Recalculate()
        {
            // Start from base
            float hp   = BaseMaxHP;
            float arm  = BaseArmor;
            float spd  = BaseSpeed;
            float trn  = BaseTurnRate;
            float acc  = BaseAcceleration;
            float cdmg = 0f, crng = 0f, cmlt = 1f;
            float tdmg = 0f, tspd = 0f;
            float acmlt = 1f, rep = 0f;

            foreach (var upg in _upgrades)
            {
                foreach (var mod in upg.bonuses)
                {
                    switch (mod.statType)
                    {
                        case StatType.MaxHP:
                            hp  = Apply(hp, mod); break;
                        case StatType.Armor:
                            arm = Apply(arm, mod); break;
                        case StatType.Speed:
                            spd = Apply(spd, mod); break;
                        case StatType.TurnRate:
                            trn = Apply(trn, mod); break;
                        case StatType.Acceleration:
                            acc = Apply(acc, mod); break;
                        case StatType.CannonDamage:
                            cdmg += mod.value; break;
                        case StatType.CannonRange:
                            crng += mod.value; break;
                        case StatType.CannonCooldown:
                            cmlt *= mod.value; break;
                        case StatType.TorpedoDamage:
                            tdmg += mod.value; break;
                        case StatType.TorpedoSpeed:
                            tspd += mod.value; break;
                        case StatType.AbilityCooldown:
                            acmlt *= mod.value; break;
                        case StatType.RepairAmount:
                            rep += mod.value; break;
                    }
                }
            }

            MaxHP               = hp;
            Armor               = Mathf.Clamp(arm, 0f, 0.75f); // cap armor at 75%
            Speed               = spd;
            TurnRate            = trn;
            Acceleration        = acc;
            CannonDamageBonus   = cdmg;
            CannonRangeBonus    = crng;
            CannonCooldownMult  = cmlt;
            TorpedoDamageBonus  = tdmg;
            TorpedoSpeedBonus   = tspd;
            AbilityCooldownMult = acmlt;
            RepairAmountBonus   = rep;
        }

        private static float Apply(float base_, StatModifier mod)
        {
            return mod.mode == ModifierMode.Additive
                ? base_ + mod.value
                : base_ * mod.value;
        }
    }
}
