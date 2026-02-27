using UnityEngine;
using IronTide.Core;

namespace IronTide.Core
{
    /// <summary>
    /// Top-level game manager. Persists across scenes via DontDestroyOnLoad.
    /// Owns the MatchStateMachine and wires up core services.
    /// Expanded fully in Day 7; stub registered on Day 1.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Register with service locator so other systems can find it
            ServiceLocator.Register<GameManager>(this);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                ServiceLocator.Unregister<GameManager>();
        }

        /// <summary>Called by Harbor when it is destroyed.</summary>
        public void DeclareWinner(int winningTeam)
        {
            Debug.Log($"[GameManager] Team {winningTeam} wins!");
            EventBus.Publish(new HarborDestroyedEvent
            {
                WinningTeam    = winningTeam,
                DestroyedTeam  = winningTeam == 0 ? 1 : 0
            });
        }
    }
}
