using UnityEngine;
using IronTide.Ships;

namespace IronTide.Economy
{
    public enum ModuleCategory { Hull, Engine, Cannon, Torpedo, Utility, Support }

    [CreateAssetMenu(fileName = "MOD_", menuName = "IronTide/Module")]
    public class ModuleData : ScriptableObject
    {
        [Header("Identity")]
        public string         moduleId;
        public string         displayName;
        [TextArea(2, 4)]
        public string         description;
        public Sprite         icon;
        public ModuleCategory category;

        [Header("Stat Modifiers")]
        public StatModifier[] statModifiers;

        [Header("Active Ability")]
        public bool           hasActiveAbility;
        // AbilityData reference added once AbilityData.cs exists

        [Header("Restrictions")]
        public ShipArchetype[] allowedArchetypes; // empty array = all archetypes
        public int             slotCost = 1;
    }
}
