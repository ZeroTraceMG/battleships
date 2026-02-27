using UnityEngine;
using IronTide.Ships;
using IronTide.Economy;

namespace IronTide.Bots
{
    public enum BotDifficulty { Easy, Medium, Hard }

    [CreateAssetMenu(fileName = "BOT_", menuName = "IronTide/Bot Tuning")]
    public class BotTuningData : ScriptableObject
    {
        [Header("Identity")]
        public string        botId;
        public BotDifficulty difficulty;
        public ShipClassData shipClass;

        [Header("Decision Weights")]
        [Range(0f, 1f)] public float aggressionBias      = 0.5f;
        [Range(0f, 1f)] public float retreatHPThreshold  = 0.25f; // retreat below 25% HP
        [Range(0f, 1f)] public float shopPriorityBias    = 0.5f;
        [Range(0f, 1f)] public float campPriorityBias    = 0.3f;

        [Header("Aim Accuracy")]
        public float aimErrorDegrees = 5f;   // random aim spread
        public float reactionTime    = 0.5f; // seconds before reacting to threat
        [Range(0f, 1f)]
        public float leadShotFactor  = 0.5f; // 0=no lead, 1=perfect lead

        [Header("Upgrade Priority")]
        public UpgradeCategory[] upgradePriority; // order bot prioritises shop categories

        [Header("Navigation")]
        public float waypointRadius  = 2f;
        public float avoidanceRadius = 5f;
    }
}
