using UnityEngine;
using IronTide.Core;
using IronTide.Ships;

namespace IronTide.Economy
{
    public enum PurchaseResult { Success, InsufficientGold, PrerequisiteNotMet, CapReached, InvalidRequest }

    /// <summary>
    /// Validates and executes shop purchases.
    /// Server-authoritative in networked mode (called only on server).
    /// Offline mode: called directly by ShopUI.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        private GoldManager _goldManager;

        private void Awake()
        {
            ServiceLocator.Register<ShopManager>(this);
        }

        private void Start()
        {
            _goldManager = ServiceLocator.Get<GoldManager>();
        }

        private void OnDestroy() => ServiceLocator.Unregister<ShopManager>();

        /// <summary>
        /// Attempt to purchase an upgrade for a player's ship.
        /// </summary>
        /// <param name="playerId">Player/network ID.</param>
        /// <param name="upgrade">The UpgradeData to purchase.</param>
        /// <param name="upgradeHandler">The ship's ShipUpgradeHandler component.</param>
        /// <param name="shipClassData">Ship class data (for upgrade cap checking).</param>
        public PurchaseResult TryPurchase(string playerId, UpgradeData upgrade,
                                          ShipUpgradeHandler upgradeHandler,
                                          ShipClassData shipClassData)
        {
            if (upgrade == null || upgradeHandler == null)
                return PurchaseResult.InvalidRequest;

            // Check upgrade cap for this ship class
            if (!IsWithinCap(upgrade, shipClassData))
                return PurchaseResult.CapReached;

            // Check prerequisite
            if (upgrade.prerequisite != null && !upgradeHandler.HasUpgrade(upgrade.prerequisite))
                return PurchaseResult.PrerequisiteNotMet;

            // Check and deduct gold
            if (!_goldManager.TrySpendGold(playerId, upgrade.goldCost))
                return PurchaseResult.InsufficientGold;

            // Apply upgrade
            upgradeHandler.AddUpgrade(upgrade);
            return PurchaseResult.Success;
        }

        private static bool IsWithinCap(UpgradeData upgrade, ShipClassData data)
        {
            if (data == null) return true;
            return upgrade.category switch
            {
                UpgradeCategory.Hull     => upgrade.level <= data.maxHullLevel,
                UpgradeCategory.Engine   => upgrade.level <= data.maxEngineLevel,
                UpgradeCategory.Cannon   => upgrade.level <= data.maxCannonLevel,
                UpgradeCategory.Torpedo  => upgrade.level <= data.maxTorpedoLevel,
                UpgradeCategory.Utility  => upgrade.level <= data.maxUtilityLevel,
                _                        => true
            };
        }
    }
}
