using System.Collections;
using UnityEngine;
using IronTide.Core;
using IronTide.Economy;

namespace IronTide.Map
{
    /// <summary>
    /// Capturable lane tower. Teams capture by standing in range.
    /// Awards gold ticks to the controlling team while held.
    /// </summary>
    public class LaneObjective : MonoBehaviour
    {
        [SerializeField] private MapObjectiveData objectiveData;
        [SerializeField] private float            captureRadius = 8f;

        public int  ControllingTeam { get; private set; } = -1; // -1 = neutral
        private int _contestingTeam = -1;
        private float _captureProgress = 0f;      // 0..1

        private GoldManager _goldManager;
        private Coroutine   _tickCoroutine;

        private void Start()
        {
            _goldManager = ServiceLocator.Get<GoldManager>();
            // Gold tick coroutine started when captured
        }

        private void Update()
        {
            DetectContesters();
            ProgressCapture();
        }

        private void DetectContesters()
        {
            // Find ships within captureRadius
            var cols = Physics.OverlapSphere(transform.position, captureRadius,
                           Utils.LayerMaskConstants.ShipsMask);
            _contestingTeam = -1;
            int team0Count = 0, team1Count = 0;

            foreach (var col in cols)
            {
                var health = col.GetComponent<Ships.ShipHealth>();
                if (health == null || health.IsDead) continue;

                var teamComp = col.GetComponent<Ships.ShipTeam>();
                if (teamComp == null) continue;

                if (teamComp.TeamId == 0) team0Count++;
                else team1Count++;
            }

            // Only contested if exactly one team present
            if (team0Count > 0 && team1Count == 0) _contestingTeam = 0;
            else if (team1Count > 0 && team0Count == 0) _contestingTeam = 1;
        }

        private void ProgressCapture()
        {
            float captureTime = objectiveData != null ? objectiveData.captureTime : 8f;

            if (_contestingTeam == -1)
            {
                // Decay capture progress back to neutral
                _captureProgress = Mathf.MoveTowards(_captureProgress, 0f, Time.deltaTime / captureTime);
                return;
            }

            if (_contestingTeam == ControllingTeam) return; // already owned, nothing to do

            _captureProgress += Time.deltaTime / captureTime;
            if (_captureProgress >= 1f)
            {
                _captureProgress = 0f;
                Capture(_contestingTeam);
            }
        }

        private void Capture(int team)
        {
            ControllingTeam = team;
            Debug.Log($"[LaneObjective] {name} captured by team {team}.");

            if (_tickCoroutine != null) StopCoroutine(_tickCoroutine);
            _tickCoroutine = StartCoroutine(GoldTickCoroutine(team));
        }

        private IEnumerator GoldTickCoroutine(int team)
        {
            float interval = objectiveData != null ? objectiveData.tickInterval : 5f;
            int   reward   = objectiveData != null ? objectiveData.tickGoldReward : 10;

            while (ControllingTeam == team)
            {
                yield return new WaitForSeconds(interval);
                // TODO: award to all team members — requires player registry
                Debug.Log($"[LaneObjective] Team {team} earns {reward}g tick.");
            }
        }
    }
}
