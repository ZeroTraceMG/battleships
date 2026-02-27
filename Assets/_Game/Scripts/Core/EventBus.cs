using System;
using System.Collections.Generic;
using UnityEngine;

namespace IronTide.Core
{
    /// <summary>
    /// Typed, static event bus. Decouples publishers from subscribers.
    /// Usage:
    ///   EventBus.Subscribe&lt;GoldChangedEvent&gt;(OnGoldChanged);
    ///   EventBus.Publish(new GoldChangedEvent { Amount = 50 });
    ///   EventBus.Unsubscribe&lt;GoldChangedEvent&gt;(OnGoldChanged);
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public static void Subscribe<T>(Action<T> handler) where T : struct
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _handlers[type] = list;
            }
            list.Add(handler);
        }

        public static void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            if (_handlers.TryGetValue(typeof(T), out var list))
                list.Remove(handler);
        }

        public static void Publish<T>(T evt) where T : struct
        {
            if (!_handlers.TryGetValue(typeof(T), out var list)) return;

            // Iterate a copy to allow safe subscribe/unsubscribe during publish.
            var snapshot = list.ToArray();
            foreach (var del in snapshot)
            {
                try
                {
                    ((Action<T>)del)(evt);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventBus] Handler threw for event {typeof(T).Name}: {ex}");
                }
            }
        }

        /// <summary>Remove all subscriptions — use in tests or full game reset.</summary>
        public static void Clear() => _handlers.Clear();
    }

    // ── Built-in game events ─────────────────────────────────────────────────

    public struct GoldChangedEvent
    {
        public string PlayerId;
        public int    NewTotal;
        public int    Delta;       // positive = earned, negative = spent
    }

    public struct ShipDiedEvent
    {
        public string VictimId;
        public string KillerId;   // empty string = environment
    }

    public struct HarborDestroyedEvent
    {
        public int DestroyedTeam;
        public int WinningTeam;
    }

    public struct MatchStateChangedEvent
    {
        public MatchState OldState;
        public MatchState NewState;
    }

    public enum MatchState
    {
        WaitingForPlayers,
        Countdown,
        Playing,
        GameOver
    }
}
