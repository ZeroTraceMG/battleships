using UnityEngine;

namespace IronTide.Map
{
    public enum ObjectiveType { Harbor, LaneObjective, NeutralCamp }

    [CreateAssetMenu(fileName = "OBJ_", menuName = "IronTide/Map Objective")]
    public class MapObjectiveData : ScriptableObject
    {
        [Header("Identity")]
        public string        objectiveId;
        public ObjectiveType type;

        [Header("HP")]
        public float maxHP = 5000f;

        [Header("Gold Rewards")]
        public int   killGoldReward   = 200;
        public int   assistGoldReward = 80;
        public int   tickGoldReward   = 10;  // per tick while controlled
        public float tickInterval     = 5f;  // seconds between gold ticks

        [Header("Capture (LaneObjective)")]
        public float captureTime = 8f;

        [Header("Neutral Camp")]
        public GameObject guardPrefab;
        public int        guardCount       = 3;
        public float      guardAggroRadius = 12f;
        public bool       respawns         = true;
        public float      respawnDelay     = 60f;
    }
}
