using System;
using System.Collections;
using UnityEngine;

namespace IronTide.Core
{
    /// <summary>
    /// Drives match lifecycle. Attach to GameManager GameObject.
    /// Transitions: WaitingForPlayers → Countdown → Playing → GameOver
    /// </summary>
    public class MatchStateMachine : MonoBehaviour
    {
        [SerializeField] private float countdownDuration = 5f;

        public MatchState CurrentState { get; private set; } = MatchState.WaitingForPlayers;

        // Raised when state changes — UI systems listen via EventBus
        public event Action<MatchState> OnStateChanged;

        public void StartCountdown()
        {
            if (CurrentState != MatchState.WaitingForPlayers) return;
            SetState(MatchState.Countdown);
            StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            yield return new WaitForSeconds(countdownDuration);
            SetState(MatchState.Playing);
        }

        public void EndMatch()
        {
            if (CurrentState != MatchState.Playing) return;
            StopAllCoroutines();
            SetState(MatchState.GameOver);
        }

        private void SetState(MatchState next)
        {
            var prev = CurrentState;
            CurrentState = next;
            OnStateChanged?.Invoke(next);
            EventBus.Publish(new MatchStateChangedEvent { OldState = prev, NewState = next });
            Debug.Log($"[MatchStateMachine] {prev} → {next}");
        }
    }
}
