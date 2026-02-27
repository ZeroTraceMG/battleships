using System.Collections.Generic;
using UnityEngine;

namespace IronTide.Map
{
    /// <summary>
    /// Drives the overhead minimap render texture camera.
    /// Ship icons and objective markers are child objects on the Minimap layer.
    /// Sonar reveals are handled via SonarRevealEvent subscription.
    /// </summary>
    public class MinimapController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera minimapCamera;
        [SerializeField] private float              minimapSize = 100f; // orthographic size

        private readonly List<MinimapIcon> _icons = new();

        private void Awake()
        {
            if (minimapCamera == null)
                Debug.LogWarning("[MinimapController] No minimap camera assigned.");
            else
                minimapCamera.orthographicSize = minimapSize;

            Core.EventBus.Subscribe<Abilities.SonarRevealEvent>(OnSonarReveal);
        }

        private void OnDestroy()
        {
            Core.EventBus.Unsubscribe<Abilities.SonarRevealEvent>(OnSonarReveal);
        }

        public void RegisterIcon(MinimapIcon icon) => _icons.Add(icon);
        public void UnregisterIcon(MinimapIcon icon) => _icons.Remove(icon);

        private void OnSonarReveal(Abilities.SonarRevealEvent evt)
        {
            // TODO: briefly show enemy ping at evt.EnemyPosition on minimap for evt.Duration seconds
            Debug.Log($"[MinimapController] Sonar reveals enemy at {evt.EnemyPosition} for {evt.Duration}s");
        }
    }

    /// <summary>Attach to any object that should appear on the minimap.</summary>
    public class MinimapIcon : MonoBehaviour
    {
        [SerializeField] private Color teamColor = Color.blue;
        public Color TeamColor => teamColor;
        public void SetTeamColor(Color c) => teamColor = c;
    }
}
