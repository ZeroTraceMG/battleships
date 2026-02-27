using UnityEngine;
using IronTide.Core;

namespace IronTide.Audio
{
    /// <summary>
    /// Central audio manager. Provides pooled AudioSource playback.
    /// Register with ServiceLocator on Awake.
    /// Phase 2+: expand with priority, ducking, and 3D spatialization.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private int   poolSize = 16;
        [SerializeField] [Range(0f,1f)] private float masterVolume = 1f;

        private AudioSource[] _pool;
        private int           _poolIndex;

        private void Awake()
        {
            _pool = new AudioSource[poolSize];
            for (int i = 0; i < poolSize; i++)
            {
                var go     = new GameObject($"AudioSource_{i}");
                go.transform.SetParent(transform);
                _pool[i]   = go.AddComponent<AudioSource>();
                _pool[i].playOnAwake = false;
            }
            ServiceLocator.Register<AudioManager>(this);
        }

        private void OnDestroy() => ServiceLocator.Unregister<AudioManager>();

        public void PlayClip(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;
            var src   = GetNextSource();
            src.clip  = clip;
            src.volume = volume * masterVolume;
            src.pitch  = pitch;
            src.transform.position = position;
            src.Play();
        }

        private AudioSource GetNextSource()
        {
            var src  = _pool[_poolIndex];
            _poolIndex = (_poolIndex + 1) % poolSize;
            return src;
        }
    }
}
