using System.Collections;
using UnityEngine;
using IronTide.Ships;
using IronTide.Economy;
using IronTide.Core;

namespace IronTide.Bots
{
    /// <summary>
    /// Simple state-machine bot AI. Drives BotSteering and BotCombatModule.
    /// Tuned via BotTuningData ScriptableObject.
    /// </summary>
    [RequireComponent(typeof(BotSteering), typeof(BotCombatModule))]
    public class BotBrain : MonoBehaviour
    {
        [SerializeField] private BotTuningData tuningData;
        [SerializeField] private Transform     enemyHarborTransform;

        public enum BotState { Patrol, Engage, Retreat, ReturnToShop, Idle }
        public BotState CurrentState { get; private set; } = BotState.Patrol;

        private ShipHealth    _health;
        private ShipStats     _stats;
        private BotSteering   _steering;
        private BotCombatModule _combat;
        private GoldManager   _goldManager;
        private string        _playerId;

        private float _stateTimer;
        private const float STATE_EVAL_INTERVAL = 0.5f;

        private void Awake()
        {
            _health   = GetComponent<ShipHealth>();
            _stats    = GetComponent<ShipStats>();
            _steering = GetComponent<BotSteering>();
            _combat   = GetComponent<BotCombatModule>();
        }

        private void Start()
        {
            _goldManager = ServiceLocator.Get<GoldManager>();
            StartCoroutine(StateMachineLoop());
        }

        private IEnumerator StateMachineLoop()
        {
            yield return new WaitForSeconds(Random.Range(0f, STATE_EVAL_INTERVAL));

            while (true)
            {
                EvaluateState();
                ExecuteState();
                yield return new WaitForSeconds(STATE_EVAL_INTERVAL);
            }
        }

        private void EvaluateState()
        {
            if (tuningData == null) return;
            float hpPct = _health.CurrentHP / _stats.MaxHP;

            // Retreat if low HP
            if (hpPct < tuningData.retreatHPThreshold && CurrentState != BotState.Retreat)
            {
                SetState(BotState.Retreat);
                return;
            }

            // Return to shop if enough gold and not in combat
            if (CurrentState != BotState.Engage && ShouldShop())
            {
                SetState(BotState.ReturnToShop);
                return;
            }

            // Otherwise engage if healthy enough
            if (hpPct >= tuningData.retreatHPThreshold)
            {
                SetState(BotState.Engage);
            }
        }

        private bool ShouldShop()
        {
            if (_goldManager == null || string.IsNullOrEmpty(_playerId)) return false;
            int gold = _goldManager.GetGold(_playerId);
            // Buy if we can afford at least one upgrade (150g minimum)
            return gold >= 150 && Random.value < tuningData.shopPriorityBias;
        }

        private void ExecuteState()
        {
            switch (CurrentState)
            {
                case BotState.Engage:
                    if (enemyHarborTransform != null)
                        _steering.MoveTo(enemyHarborTransform.position);
                    _combat.TryAttack();
                    break;

                case BotState.Retreat:
                    // TODO: move toward own harbor (Phase 2 — needs own harbor ref)
                    _combat.StopAttacking();
                    break;

                case BotState.ReturnToShop:
                    // TODO: move to own harbor, trigger shop purchase (Phase 2)
                    SetState(BotState.Patrol);
                    break;

                case BotState.Patrol:
                    _combat.TryAttack();
                    break;
            }
        }

        private void SetState(BotState next)
        {
            if (CurrentState == next) return;
            CurrentState = next;
        }

        public void SetPlayerId(string id) => _playerId = id;
        public void SetEnemyHarbor(Transform t) => enemyHarborTransform = t;
    }
}
