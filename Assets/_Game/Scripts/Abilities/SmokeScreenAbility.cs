using System.Collections;
using UnityEngine;

namespace IronTide.Abilities
{
    /// <summary>
    /// Deploys a smoke cloud at the ship's position.
    /// Ships inside take an aim-accuracy penalty (applied via AimDebuffZone component).
    /// Minimap vision is also blocked within the zone.
    /// </summary>
    public class SmokeScreenAbility : AbilityBase
    {
        [SerializeField] private GameObject smokeCloudPrefab;
        [SerializeField] private float      smokeRadius = 8f;

        protected override void Activate()
        {
            if (smokeCloudPrefab != null)
            {
                var cloud = Instantiate(smokeCloudPrefab, transform.position, Quaternion.identity);
                var zone  = cloud.GetComponent<AimDebuffZone>();
                if (zone != null) zone.Init(smokeRadius, duration);
                Destroy(cloud, duration + 0.5f);
            }
        }
    }

    /// <summary>
    /// Placeholder: applies accuracy debuff to ships within radius each tick.
    /// Full implementation in Phase 2 (requires AimDebuff stat pipeline).
    /// </summary>
    public class AimDebuffZone : MonoBehaviour
    {
        private float _radius, _duration;

        public void Init(float radius, float dur) { _radius = radius; _duration = dur; }
    }
}
