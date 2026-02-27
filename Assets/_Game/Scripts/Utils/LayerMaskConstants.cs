namespace IronTide.Utils
{
    /// <summary>
    /// Central registry for all Unity layer names and their computed masks.
    /// Add a layer name here when you create it in Project Settings → Tags and Layers.
    /// </summary>
    public static class LayerMaskConstants
    {
        // Layer names (must match Project Settings exactly)
        public const string Default      = "Default";
        public const string Water        = "Water";
        public const string Ships        = "Ships";
        public const string Projectiles  = "Projectiles";
        public const string MapObjects   = "MapObjects";    // Harbors, Objectives, Camps
        public const string UI           = "UI";
        public const string Minimap      = "Minimap";

        // Pre-computed masks (call after scene load; Unity layers are set at runtime)
        public static int ShipsMask       => UnityEngine.LayerMask.GetMask(Ships);
        public static int ProjectilesMask => UnityEngine.LayerMask.GetMask(Projectiles);
        public static int MapObjectsMask  => UnityEngine.LayerMask.GetMask(MapObjects);
        public static int HittableMask    => UnityEngine.LayerMask.GetMask(Ships, MapObjects);
    }
}
