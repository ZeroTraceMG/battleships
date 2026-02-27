using System.Collections.Generic;
using UnityEngine;
using IronTide.Core;

namespace IronTide.Economy
{
    /// <summary>
    /// Server-authoritative gold ledger. In offline mode runs on the client.
    /// In networked mode only the server calls AddGold/SpendGold directly;
    /// clients receive SyncVar updates (handled in NetworkGoldSync).
    /// </summary>
    public class GoldManager : MonoBehaviour
    {
        private readonly Dictionary<string, int> _gold = new();

        private void Awake()
        {
            ServiceLocator.Register<GoldManager>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<GoldManager>();
        }

        public int GetGold(string playerId) =>
            _gold.TryGetValue(playerId, out int g) ? g : 0;

        /// <summary>Add gold to a player's balance. Server-only in networked mode.</summary>
        public void AddGold(string playerId, int amount)
        {
            if (amount <= 0) return;
            _gold.TryGetValue(playerId, out int current);
            _gold[playerId] = current + amount;
            NotifyChanged(playerId, amount);
        }

        /// <summary>
        /// Attempt to spend gold. Returns true and deducts on success.
        /// Returns false (no deduction) if balance insufficient.
        /// </summary>
        public bool TrySpendGold(string playerId, int amount)
        {
            if (amount <= 0) return true;
            int balance = GetGold(playerId);
            if (balance < amount) return false;

            _gold[playerId] = balance - amount;
            NotifyChanged(playerId, -amount);
            return true;
        }

        private void NotifyChanged(string playerId, int delta)
        {
            EventBus.Publish(new GoldChangedEvent
            {
                PlayerId = playerId,
                NewTotal = _gold[playerId],
                Delta    = delta
            });
        }
    }
}
