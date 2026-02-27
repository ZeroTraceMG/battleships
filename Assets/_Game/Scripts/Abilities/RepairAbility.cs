using System.Collections;
using UnityEngine;

namespace IronTide.Abilities
{
    /// <summary>
    /// Heal-over-time active ability. Heals the caster over 'duration' seconds.
    /// Healing amount is boosted by ShipStats.RepairAmountBonus (upgrade system).
    /// </summary>
    [RequireComponent(typeof(Ships.ShipHealth))]
    public class RepairAbility : AbilityBase
    {
        [SerializeField] private float healPerSecond = 30f;
        [SerializeField] private GameObject repairVFXPrefab;

        private Ships.ShipHealth _health;

        protected override void Awake()
        {
            base.Awake();
            _health = GetComponent<Ships.ShipHealth>();
        }

        protected override void Activate()
        {
            StartCoroutine(HealCoroutine());

            if (repairVFXPrefab != null)
            {
                var vfx = Instantiate(repairVFXPrefab, transform.position, Quaternion.identity);
                vfx.transform.SetParent(transform);
                Destroy(vfx, duration);
            }
        }

        private IEnumerator HealCoroutine()
        {
            float elapsed  = 0f;
            float hps      = healPerSecond + (Stats != null ? Stats.RepairAmountBonus : 0f);

            while (elapsed < duration)
            {
                float dt = Time.deltaTime;
                _health.Heal(hps * dt);
                elapsed += dt;
                yield return null;
            }
        }
    }
}
