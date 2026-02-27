using System.Collections.Generic;
using UnityEngine;

namespace IronTide.Utils
{
    /// <summary>
    /// Generic MonoBehaviour-based object pool.
    /// Place one in the scene per pooled prefab type.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int        initialSize = 20;

        private readonly Queue<GameObject> _free = new();

        private void Awake() => Prewarm();

        private void Prewarm()
        {
            for (int i = 0; i < initialSize; i++)
                _free.Enqueue(CreateInstance());
        }

        private GameObject CreateInstance()
        {
            var go = Instantiate(prefab, transform);
            go.SetActive(false);
            return go;
        }

        /// <summary>Retrieve an active instance from the pool (or create a new one).</summary>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject go = _free.Count > 0 ? _free.Dequeue() : CreateInstance();
            go.transform.SetPositionAndRotation(position, rotation);
            go.SetActive(true);
            return go;
        }

        /// <summary>Return an instance to the pool.</summary>
        public void Return(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(transform);
            _free.Enqueue(go);
        }

        // ── Static helpers ───────────────────────────────────────────────

        private static readonly Dictionary<GameObject, ObjectPool> _poolMap = new();

        /// <summary>
        /// Register a pool so projectiles can return themselves without a direct reference.
        /// Called automatically by ObjectPool.Awake if prefab is set.
        /// </summary>
        public static void RegisterPool(GameObject prefab, ObjectPool pool) => _poolMap[prefab] = pool;

        public static void ReturnToPool(GameObject prefab, GameObject instance)
        {
            if (_poolMap.TryGetValue(prefab, out var pool))
                pool.Return(instance);
            else
                Destroy(instance);
        }
    }
}
