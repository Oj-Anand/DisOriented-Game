using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    [CreateAssetMenu(fileName = "PacemogConfig", menuName = "DisOriented/Pacemog Config")]
    public class RunnerConfig : ScriptableObject
    {
        [Header("Lane Layout")]
        public LaneDefinition[] lanes = new LaneDefinition[]
        {
            new LaneDefinition { xPosition = -3f, isOncoming = true,
                scoreMultiplier = 2f, laneName = "Oncoming Far" },
            new LaneDefinition { xPosition = -1f, isOncoming = true,
                scoreMultiplier = 1.5f, laneName = "Oncoming Near" },
            new LaneDefinition { xPosition = 1f, isOncoming = false,
                scoreMultiplier = 1f, laneName = "Same Near" },
            new LaneDefinition { xPosition = 3f, isOncoming = false,
                scoreMultiplier = 1f, laneName = "Same Far" },
        };

        [Header("Scrolling")]
        public float baseScrollSpeed = 5f;
        [Tooltip("How much faster oncoming traffic moves relative to road")]
        public float oncomingSpeedMultiplier = 1.8f;
        [Tooltip("Same-direction traffic moves slower than the player")]
        public float sameDirectionSpeedMultiplier = 0.6f;

        [Header("Timer")]
        public float runDurationSeconds = 60f;

        [Header("Player Movement")]
        public float laneSwitchDuration = 0.15f;
        public int startingLaneIndex = 2; //Right-side safe lane

        [Header("Ice Effect")]
        [Tooltip("Multiplier applied to laneSwitchDuration when on ice")]
        public float iceSlowMultiplier = 3f;
        public float iceEffectDuration = 1.5f;

        [Header("Spawning")]
        public float baseSpawnInterval = 1.2f;
        [Tooltip("Min seconds between spawns at max difficulty")]
        public float minSpawnInterval = 0.4f;
        public float coinSpawnChance = 0.25f;
        public float iceSpawnChance = 0.15f;

        [Header("Difficulty Ramp")]
        [Tooltip("Scroll speed multiplier at end of run")]
        public float maxSpeedMultiplier = 1.8f;
        [Tooltip("Spawn interval multiplier at end of run (lower = more spawns)")]
        public float maxSpawnRateMultiplier = 0.4f;

        [Header("Scoring")]
        public int pointsPerPedestrianPassed = 100;
        public int coinValue = 25;

        [Header("Resource Rewards / Penalties")]
        public float successMoodReward = 15f;
        public float successSwagReward = 10f;
        [Tooltip("Applied per 1000 score points on success")]
        public float bonusMoodPerThousandPoints = 5f;
        public float bonusSwagPerThousandPoints = 3f;
        public float gameOverEnergyPenalty = -8f;
        public float gameOverTummyPenalty = -5f;

        [Header("Object Pooling")]
        public int pedestrianPoolSize = 20;
        public int icePoolSize = 10;
        public int coinPoolSize = 15;

        [Header("World Bounds")]
        public float spawnY = 12f;   //Top of screen spawn point
        public float despawnY = -8f;  //Below screen destroy point

    }
}

