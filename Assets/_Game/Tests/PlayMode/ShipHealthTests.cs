using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using IronTide.Ships;
using IronTide.Economy;

namespace IronTide.Tests.PlayMode
{
    /// <summary>Play Mode tests for ShipHealth — require scene context (GameObject).</summary>
    public class ShipHealthTests
    {
        private GameObject _shipGO;
        private ShipStats  _stats;
        private ShipHealth _health;

        [SetUp]
        public void SetUp()
        {
            _shipGO = new GameObject("TestShip");
            _stats  = _shipGO.AddComponent<ShipStats>();
            _health = _shipGO.AddComponent<ShipHealth>();

            // Manually init stats with a mock ShipClassData
            var data             = ScriptableObject.CreateInstance<ShipClassData>();
            data.baseMaxHP       = 1000f;
            data.baseArmor       = 0f;
            data.baseSpeed       = 10f;
            data.baseTurnRate    = 90f;
            data.baseAcceleration = 15f;
            _stats.InitFromData(data);
            _health.InitialiseHP("player1");
        }

        [TearDown]
        public void TearDown() => Object.Destroy(_shipGO);

        [Test]
        public void TakeDamage_ReducesHP()
        {
            _health.TakeDamage(200f);
            Assert.AreEqual(800f, _health.CurrentHP, 0.01f);
        }

        [Test]
        public void Armor_ReducesDamage()
        {
            // Set 50% armor via a stat modifier
            var upg = ScriptableObject.CreateInstance<UpgradeData>();
            upg.bonuses = new[] { new StatModifier
                { statType = StatType.Armor, mode = ModifierMode.Additive, value = 0.5f } };
            _stats.AddUpgrade(upg);

            _health.TakeDamage(200f);
            Assert.AreEqual(900f, _health.CurrentHP, 0.01f); // 200 * (1-0.5) = 100 damage
        }

        [Test]
        public void Death_FiresExactlyOnce()
        {
            int deathCount = 0;
            _health.OnDeath.AddListener(() => deathCount++);

            _health.TakeDamage(2000f); // overkill
            _health.TakeDamage(100f);  // dead already

            Assert.AreEqual(1, deathCount);
        }

        [Test]
        public void Heal_RestoresHP()
        {
            _health.TakeDamage(300f);
            _health.Heal(100f);
            Assert.AreEqual(800f, _health.CurrentHP, 0.01f);
        }

        [Test]
        public void Heal_DoesNotExceedMaxHP()
        {
            _health.Heal(500f);
            Assert.AreEqual(1000f, _health.CurrentHP, 0.01f);
        }

        [UnityTest]
        public IEnumerator TurnRate_MatchesShipClassData()
        {
            Assert.AreEqual(90f, _stats.TurnRate, 0.01f);
            yield return null;
        }
    }
}
