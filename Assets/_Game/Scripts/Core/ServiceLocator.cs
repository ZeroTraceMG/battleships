using System;
using System.Collections.Generic;
using UnityEngine;

namespace IronTide.Core
{
    /// <summary>
    /// Static service registry. Register services at startup (Bootstrap scene);
    /// retrieve them anywhere without coupling to a specific MonoBehaviour.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>Register a service instance under type T.</summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                Debug.LogWarning($"[ServiceLocator] Overwriting existing service of type {type.Name}.");
            }
            _services[type] = service;
        }

        /// <summary>Retrieve a registered service. Logs error if not found.</summary>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var service))
                return service as T;

            Debug.LogError($"[ServiceLocator] Service of type {type.Name} is not registered.");
            return null;
        }

        /// <summary>Returns true if a service of type T is registered.</summary>
        public static bool Has<T>() where T : class => _services.ContainsKey(typeof(T));

        /// <summary>Unregister a service (call on scene unload if needed).</summary>
        public static void Unregister<T>() where T : class => _services.Remove(typeof(T));

        /// <summary>Clear all services — use in tests or full game reset.</summary>
        public static void Clear() => _services.Clear();
    }
}
