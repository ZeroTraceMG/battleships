using System.Collections.Generic;
using UnityEngine;
using IronTide.Core;

namespace IronTide.Economy
{
    /// <summary>
    /// Tracks damage dealt per attacker per target.
    /// On ShipDiedEvent: awards kill gold to killer, assist gold to qualifying helpers.
    /// </summary>
    public class KillTracker : MonoBehaviour
    {
        [Header("Gold rewards")]
        [SerializeField] private int   killGoldBase   = 50;  // × ship tier
        [SerializeField] private float assistGoldMult = 0.4f;
        [SerializeField] private float assistWindow   = 10f; // seconds

        [Header("Minimum damage % for assist")]
        [SerializeField] private float assistMinDmgPct = 0.10f;

        private struct DamageRecord
        {
            public float Amount;
            public float Timestamp;
        }

        // [victimId][attackerId] = list of damage records
        private readonly Dictionary<string, Dictionary<string, List<DamageRecord>>> _records = new();

        private GoldManager _goldManager;

        private void Awake()
        {
            ServiceLocator.Register<KillTracker>(this);
        }

        private void Start()
        {
            _goldManager = ServiceLocator.Get<GoldManager>();
            EventBus.Subscribe<ShipDiedEvent>(OnShipDied);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<ShipDiedEvent>(OnShipDied);
            ServiceLocator.Unsubscribe<KillTracker>();
        }

        /// <summary>Call whenever a ship deals damage to another.</summary>
        public void RecordDamage(string attackerId, string victimId, float amount)
        {
            if (!_records.TryGetValue(victimId, out var byAttacker))
            {
                byAttacker      = new Dictionary<string, List<DamageRecord>>();
                _records[victimId] = byAttacker;
            }
            if (!byAttacker.TryGetValue(attackerId, out var list))
            {
                list               = new List<DamageRecord>();
                byAttacker[attackerId] = list;
            }
            list.Add(new DamageRecord { Amount = amount, Timestamp = Time.time });
        }

        private void OnShipDied(ShipDiedEvent evt)
        {
            AwardGold(evt.VictimId, evt.KillerId, killGoldBase * GetTier(evt.VictimId));
            _records.Remove(evt.VictimId);
        }

        private void AwardGold(string victimId, string killerId, int killGold)
        {
            if (_goldManager == null) return;

            // Killer gold
            if (!string.IsNullOrEmpty(killerId))
                _goldManager.AddGold(killerId, killGold);

            // Assist gold: any attacker with >assistMinDmgPct total dmg in last assistWindow seconds
            if (!_records.TryGetValue(victimId, out var byAttacker)) return;

            float totalRecent = 0f;
            float cutoff      = Time.time - assistWindow;

            // First pass: sum total recent damage
            var recentPerAttacker = new Dictionary<string, float>();
            foreach (var (attId, records) in byAttacker)
            {
                float sum = 0f;
                foreach (var r in records)
                    if (r.Timestamp >= cutoff) sum += r.Amount;
                recentPerAttacker[attId] = sum;
                totalRecent += sum;
            }

            if (totalRecent <= 0f) return;

            foreach (var (attId, dmg) in recentPerAttacker)
            {
                if (attId == killerId) continue; // killer already got full gold
                if (dmg / totalRecent >= assistMinDmgPct)
                    _goldManager.AddGold(attId, Mathf.RoundToInt(killGold * assistGoldMult));
            }
        }

        // Temporary: tier lookup until ShipClassData is accessible from ShipHealth
        private static int GetTier(string victimId) => 1; // TODO: resolve from active ship dict
    }
}
