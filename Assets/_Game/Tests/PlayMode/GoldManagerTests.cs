using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using IronTide.Economy;
using IronTide.Core;

namespace IronTide.Tests.PlayMode
{
    /// <summary>Play Mode tests for GoldManager — requires scene context.</summary>
    public class GoldManagerTests
    {
        private GameObject   _managerGO;
        private GoldManager  _goldManager;

        [SetUp]
        public void SetUp()
        {
            ServiceLocator.Clear();
            EventBus.Clear();
            _managerGO   = new GameObject("GoldManager");
            _goldManager = _managerGO.AddComponent<GoldManager>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.Destroy(_managerGO);
            ServiceLocator.Clear();
            EventBus.Clear();
        }

        [Test]
        public void AddGold_IncreasesBalance()
        {
            _goldManager.AddGold("p1", 150);
            Assert.AreEqual(150, _goldManager.GetGold("p1"));
        }

        [Test]
        public void TrySpendGold_SucceedsWhenSufficient()
        {
            _goldManager.AddGold("p1", 300);
            bool success = _goldManager.TrySpendGold("p1", 150);
            Assert.IsTrue(success);
            Assert.AreEqual(150, _goldManager.GetGold("p1"));
        }

        [Test]
        public void TrySpendGold_FailsWhenInsufficient()
        {
            _goldManager.AddGold("p1", 50);
            bool success = _goldManager.TrySpendGold("p1", 150);
            Assert.IsFalse(success);
            Assert.AreEqual(50, _goldManager.GetGold("p1")); // unchanged
        }

        [Test]
        public void AddGold_PublishesGoldChangedEvent()
        {
            GoldChangedEvent received = default;
            EventBus.Subscribe<GoldChangedEvent>(e => received = e);

            _goldManager.AddGold("p1", 200);

            Assert.AreEqual("p1",  received.PlayerId);
            Assert.AreEqual(200,   received.NewTotal);
            Assert.AreEqual(200,   received.Delta);

            EventBus.Unsubscribe<GoldChangedEvent>(e => received = e);
        }

        [UnityTest]
        public IEnumerator SpendGold_AfterPurchase_StatModApplied()
        {
            // Verify the purchase → stat pipeline works end-to-end
            // (ShopManager → GoldManager → ShipUpgradeHandler → ShipStats)
            var shipGO   = new GameObject("Ship");
            var stats    = shipGO.AddComponent<Ships.ShipStats>();
            var handler  = shipGO.AddComponent<Ships.ShipUpgradeHandler>();

            var classData              = ScriptableObject.CreateInstance<Ships.ShipClassData>();
            classData.baseMaxHP        = 1000f;
            classData.baseArmor        = 0f;
            classData.baseSpeed        = 10f;
            classData.baseTurnRate     = 90f;
            classData.baseAcceleration = 15f;
            stats.InitFromData(classData);

            _goldManager.AddGold("p1", 500);

            var shopManagerGO = new GameObject("ShopManager");
            var shopManager   = shopManagerGO.AddComponent<Economy.ShopManager>();
            yield return null; // let Start() run

            var upgrade      = ScriptableObject.CreateInstance<UpgradeData>();
            upgrade.level    = 1;
            upgrade.goldCost = 150;
            upgrade.category = UpgradeCategory.Hull;
            upgrade.bonuses  = new[] { new StatModifier
                { statType = StatType.MaxHP, mode = ModifierMode.Additive, value = 100f } };

            var result = shopManager.TryPurchase("p1", upgrade, handler, classData);

            Assert.AreEqual(PurchaseResult.Success, result);
            Assert.AreEqual(1100f, stats.MaxHP, 0.01f);
            Assert.AreEqual(350,   _goldManager.GetGold("p1"));

            Object.Destroy(shipGO);
            Object.Destroy(shopManagerGO);
        }
    }
}
