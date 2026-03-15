using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    public class DifficultyManager : MonoBehaviour
    {
        private RunnerConfig _config;
        private ObstacleSpawner _spawner;
        private RoadScroller _road;
        private float _runDuration;
        private float _elapsed;

        public float DifficultyProgress => Mathf.Clamp01(_elapsed / _runDuration);
        public float CurrentSpeedMultiplier { get; private set; } = 1f;

        public void Initialize(RunnerConfig cfg, ObstacleSpawner spawner, RoadScroller road)
        {
            _config = cfg;
            _spawner = spawner;
            _road = road;
            _runDuration = cfg.runDurationSeconds;
            _elapsed = 0f;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = DifficultyProgress;

            //lerp speed: 1x -> maxSpeedMultiplier
            CurrentSpeedMultiplier = Mathf.Lerp(1f, _config.maxSpeedMultiplier, t);

            //lerp spawn rate: 1x -> maxSpawnRateMultiplier (lower = more frequent)
            float spawnMult = Mathf.Lerp(1f, _config.maxSpawnRateMultiplier, t);

            _spawner.SetDifficultyParams(CurrentSpeedMultiplier, spawnMult);
            _road.SetScrollSpeed(_config.baseScrollSpeed * CurrentSpeedMultiplier);
        }


    }
}

