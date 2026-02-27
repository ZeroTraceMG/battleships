using UnityEngine;
using IronTide.Ships;

namespace IronTide.Abilities
{
    /// <summary>
    /// Base class for active ship abilities (Smoke, Sonar, Repair, etc.).
    /// Attach to the ship GameObject alongside CannonWeapon/TorpedoWeapon.
    /// </summary>
    public abstract class AbilityBase : MonoBehaviour
    {
        [SerializeField] protected float cooldown = 15f;
        [SerializeField] protected float duration  = 5f;

        private float _cooldownRemaining;
        protected ShipStats Stats;

        protected virtual void Awake()
        {
            Stats = GetComponent<ShipStats>();
        }

        protected virtual void Update()
        {
            if (_cooldownRemaining > 0f)
                _cooldownRemaining -= Time.deltaTime;
        }

        public bool CanActivate => _cooldownRemaining <= 0f;

        public bool TryActivate()
        {
            if (!CanActivate) return false;

            float effectiveCooldown = cooldown;
            if (Stats != null) effectiveCooldown *= Stats.AbilityCooldownMult;
            _cooldownRemaining = effectiveCooldown;

            Activate();
            return true;
        }

        protected abstract void Activate();

        public float CooldownRemaining => _cooldownRemaining;
        public float CooldownDuration  => cooldown;
    }
}
