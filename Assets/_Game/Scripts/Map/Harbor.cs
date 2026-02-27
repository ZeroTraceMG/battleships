using UnityEngine;
using UnityEngine.Events;
using IronTide.Core;

namespace IronTide.Map
{
    /// <summary>
    /// Harbor entity: team spawn point, shop zone, and win-condition objective.
    /// Assign teamId in Inspector (0 = blue team, 1 = red team).
    /// The harbor can only be damaged by the opposing team's projectiles.
    /// </summary>
    public class Harbor : MonoBehaviour
    {
        [Header("Team")]
        [SerializeField] public int teamId = 0;

        [Header("HP")]
        [SerializeField] private MapObjectiveData objectiveData;
        private float _currentHP;
        public  float CurrentHP => _currentHP;
        public  bool  IsDestroyed { get; private set; }

        [HideInInspector] public UnityEvent OnDestroyed;

        private void Awake()
        {
            _currentHP  = objectiveData != null ? objectiveData.maxHP : 5000f;
            IsDestroyed = false;
        }

        /// <summary>
        /// Deal damage to the harbor. Only the opposing team can damage it.
        /// </summary>
        public void TakeDamage(float amount, int attackerTeam, string attackerId = "")
        {
            if (IsDestroyed) return;
            if (attackerTeam == teamId) return;   // friendly fire ignored

            _currentHP = Mathf.Max(0f, _currentHP - amount);

            if (_currentHP <= 0f)
                Destroy_(attackerId);
        }

        private void Destroy_(string lastAttackerId)
        {
            if (IsDestroyed) return;
            IsDestroyed = true;

            int winningTeam = teamId == 0 ? 1 : 0;
            Debug.Log($"[Harbor] Team {teamId} harbor destroyed. Team {winningTeam} wins!");

            var gm = ServiceLocator.Get<GameManager>();
            gm?.DeclareWinner(winningTeam);

            OnDestroyed?.Invoke();
        }

        /// <summary>True if this is the shop zone for the given team.</summary>
        public bool IsShopZoneForTeam(int team) => team == teamId;
    }
}
