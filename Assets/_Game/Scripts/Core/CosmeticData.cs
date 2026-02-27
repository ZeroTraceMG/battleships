using UnityEngine;

namespace IronTide.Core
{
    public enum CosmeticType   { ShipSkin, Trail, Decal, Horn, Nameplate }
    public enum Rarity         { Common, Rare, Epic, Legendary }
    public enum CosmeticSource { Earnable, BattlePass, DirectPurchase }

    [CreateAssetMenu(fileName = "COS_", menuName = "IronTide/Cosmetic")]
    public class CosmeticData : ScriptableObject
    {
        [Header("Identity")]
        public string       cosmeticId;
        public string       displayName;
        public CosmeticType type;
        public Rarity       rarity;
        public Sprite       previewImage;

        [Header("Applied Assets")]
        public Material     skinMaterial;   // ShipSkin
        public GameObject   trailPrefab;    // Trail
        public Texture2D    decalTexture;   // Decal
        public AudioClip    hornClip;       // Horn

        [Header("Unlock")]
        public CosmeticSource source;
        public int            earnCurrencyCost;

        // Enforced: cosmetics MUST have zero gameplay impact.
        // An automated test verifies this field is always false.
        [SerializeField, HideInInspector]
        private bool _hasGameplayImpact = false;
    }
}
