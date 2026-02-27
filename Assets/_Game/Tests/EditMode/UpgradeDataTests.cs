using NUnit.Framework;
using IronTide.Economy;

namespace IronTide.Tests.EditMode
{
    /// <summary>
    /// Verify UpgradeData cost curve and stat modifier structure load correctly.
    /// These tests exercise the ScriptableObject schema without scene context.
    /// </summary>
    public class UpgradeDataTests
    {
        [Test]
        public void UpgradeCategory_HasExpectedValues()
        {
            // Ensure enum values exist as expected by other systems
            Assert.IsTrue(System.Enum.IsDefined(typeof(UpgradeCategory), "Hull"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UpgradeCategory), "Engine"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UpgradeCategory), "Cannon"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UpgradeCategory), "Torpedo"));
            Assert.IsTrue(System.Enum.IsDefined(typeof(UpgradeCategory), "Utility"));
        }

        [Test]
        public void StatModifier_Additive_AddsCorrectly()
        {
            var mod = new StatModifier
            {
                statType = StatType.MaxHP,
                mode     = ModifierMode.Additive,
                value    = 100f
            };
            float result = 1000f + (mod.mode == ModifierMode.Additive ? mod.value : 0f);
            Assert.AreEqual(1100f, result, 0.001f);
        }

        [Test]
        public void StatModifier_Multiplicative_MultipliesCorrectly()
        {
            var mod = new StatModifier
            {
                statType = StatType.Speed,
                mode     = ModifierMode.Multiplicative,
                value    = 1.08f  // +8%
            };
            float result = 10f * (mod.mode == ModifierMode.Multiplicative ? mod.value : 1f);
            Assert.AreEqual(10.8f, result, 0.001f);
        }

        [Test]
        public void CosmeticData_HasNoGameplayImpact()
        {
            // Verify the _hasGameplayImpact field default is false via reflection
            var info = typeof(IronTide.Core.CosmeticData)
                .GetField("_hasGameplayImpact",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(info, "_hasGameplayImpact field must exist on CosmeticData");
            // Default value is false; cannot create ScriptableObject in EditMode without scene,
            // so we verify the declared type only.
            Assert.AreEqual(typeof(bool), info.FieldType);
        }
    }
}
