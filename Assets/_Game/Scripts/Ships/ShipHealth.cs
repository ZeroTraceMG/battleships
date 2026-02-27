using UnityEngine;
using UnityEngine.Events;
using IronTide.Core;

namespace IronTide.Ships
{
    /// <summary>
    /// Manages a ship's hit points. Server-authoritative in networked mode.
    /// Raises OnDeath exactly once. Emit ShipDiedEvent via EventBus for economy/kill tracking.
    /// </summary>
    [RequireComponent(typeof(ShipStats))]
    public class ShipHealth : MonoBehaviour
    {
        public float CurrentHP { get; private set; }
        public bool  IsDead    { get; private set; }

        [HideInInspector] public UnityEvent OnDeath;

        private ShipStats _stats;
        private string    _shipOwnerId;   // set by GameManager on spawn

        private void Awake()
        {
            _stats = GetComponent<ShipStats>();
        }

        public void InitialiseHP(string ownerId)
        {
            _shipOwnerId = ownerId;
            CurrentHP    = _stats.MaxHP;
            IsDead       = false;
        }

        /// <param name="rawDamage">Damage before armor reduction.</param>
        /// <param name="attackerId">Network/player ID of attacker (for kill attribution).</param>
        public void TakeDamage(float rawDamage, string attackerId = "")
        {
            if (IsDead) return;

            float reduced = rawDamage * (1f - _stats.Armor);
            CurrentHP     = Mathf.Max(0f, CurrentHP - reduced);

            if (CurrentHP <= 0f)
                Die(attackerId);
        }

        public void Heal(float amount)
        {
            if (IsDead) return;
            CurrentHP = Mathf.Min(_stats.MaxHP, CurrentHP + amount);
        }

        private void Die(string killerId)
        {
            if (IsDead) return;
            IsDead = true;

            EventBus.Publish(new ShipDiedEvent
            {
                VictimId = _shipOwnerId,
                KillerId = killerId
            });

            OnDeath?.Invoke();
        }
    }
}
