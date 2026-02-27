using UnityEngine;
using IronTide.Core;

namespace IronTide.Abilities
{
    /// <summary>
    /// Broadcasts a Sonar pulse that reveals enemy positions within radius.
    /// Revealed ships are shown on the minimap for the full duration.
    /// </summary>
    public class SonarAbility : AbilityBase
    {
        [SerializeField] private float revealRadius = 25f;
        [SerializeField] private GameObject sonarRingVFXPrefab;

        protected override void Activate()
        {
            if (sonarRingVFXPrefab != null)
            {
                var vfx = Instantiate(sonarRingVFXPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 2f);
            }

            // Find enemies in radius and broadcast reveal event
            var hits = Physics.OverlapSphere(transform.position, revealRadius,
                           Utils.LayerMaskConstants.ShipsMask);

            var ownerTeam = GetComponent<Ships.ShipTeam>();
            int myTeam    = ownerTeam != null ? ownerTeam.TeamId : -1;

            foreach (var col in hits)
            {
                var team = col.GetComponent<Ships.ShipTeam>();
                if (team == null || team.TeamId == myTeam) continue;

                EventBus.Publish(new SonarRevealEvent
                {
                    EnemyPosition = col.transform.position,
                    EnemyId       = col.gameObject.name,
                    Duration      = duration
                });
            }
        }
    }

    public struct SonarRevealEvent
    {
        public Vector3 EnemyPosition;
        public string  EnemyId;
        public float   Duration;
    }
}
