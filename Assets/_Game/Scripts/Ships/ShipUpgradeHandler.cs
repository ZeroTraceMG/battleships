using System.Collections.Generic;
using UnityEngine;
using IronTide.Economy;

namespace IronTide.Ships
{
    /// <summary>
    /// Stores purchased upgrades for a ship and triggers ShipStats recalculation.
    /// One instance per ship. ShopManager calls AddUpgrade() after validating the purchase.
    /// </summary>
    [RequireComponent(typeof(ShipStats))]
    public class ShipUpgradeHandler : MonoBehaviour
    {
        private ShipStats                         _stats;
        private readonly List<UpgradeData>        _purchased = new();
        private readonly Dictionary<UpgradeCategory, int> _levels  = new();

        private void Awake() => _stats = GetComponent<ShipStats>();

        public int GetLevel(UpgradeCategory category) =>
            _levels.TryGetValue(category, out int lvl) ? lvl : 0;

        public bool HasUpgrade(UpgradeData upgrade) => _purchased.Contains(upgrade);

        /// <summary>
        /// Apply upgrade. Validates prerequisite. Returns false if invalid.
        /// </summary>
        public bool AddUpgrade(UpgradeData upgrade)
        {
            if (upgrade == null) return false;

            // Prerequisite check
            if (upgrade.prerequisite != null && !_purchased.Contains(upgrade.prerequisite))
            {
                Debug.LogWarning($"[ShipUpgradeHandler] Prerequisite not met for {upgrade.upgradeId}.");
                return false;
            }

            _purchased.Add(upgrade);
            _levels[upgrade.category] = upgrade.level;

            _stats.AddUpgrade(upgrade);
            return true;
        }

        public IReadOnlyList<UpgradeData> PurchasedUpgrades => _purchased;
    }
}
