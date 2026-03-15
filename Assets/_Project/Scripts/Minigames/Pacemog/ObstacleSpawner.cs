using System.Collections.Generic;
using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    /// <summary>Spawns obstacles using object pools</summary>
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private RunnerConfig config;

        [Header("Prefabs")]
        [SerializeField] private GameObject pedestrianPrefab;
        [SerializeField] private GameObject icePrefab;
        [SerializeField] private GameObject coinPrefab;

        //=========== POOLS ===========
        private Queue<ObstacleBase> _pedestrianPool;
        private Queue<ObstacleBase> _icePool;
        private Queue<ObstacleBase> _coinPool;

        //=========== STATE =============
        private float _spawnTimer;
        private float _currentSpawnInterval;
        private float _currentSpeedMultiplier = 1f;
        private bool _isActive;

        //=========== EVENTS ============   
        public event System.Action<Pedestrian> OnPedestrianPassed;
        public event System.Action<int> OnCoinCollected;

        public void Initialize(RunnerConfig cfg)
        {
            config = cfg;
            BuildPools();
            _currentSpawnInterval = config.baseSpawnInterval;
            _isActive = true;
        }

        public void SetDifficultyParams(
            float speedMultiplier, float spawnRateMultiplier)
        {
            _currentSpeedMultiplier = speedMultiplier;
            _currentSpawnInterval = config.baseSpawnInterval * spawnRateMultiplier;
            _currentSpawnInterval = Mathf.Max(
                _currentSpawnInterval, config.minSpawnInterval);
        }

        public void StopSpawning() => _isActive = false;

        private void Update()
        {
            if (!_isActive) return;

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0f)
            {
                SpawnWave();
                _spawnTimer = _currentSpawnInterval;
            }
        }

        private void SpawnWave()
        {
            //Pick a random lane for the pedestrian
            int lane = Random.Range(0, config.lanes.Length);
            SpawnPedestrian(lane);

            //chance to spawn ice on a different lane
            if (Random.value < config.iceSpawnChance)
            {
                int iceLane = Random.Range(0, config.lanes.Length);
                SpawnIce(iceLane);
            }

            //chance to spawn coin on a different lane
            if (Random.value < config.coinSpawnChance)
            {
                int coinLane = Random.Range(0, config.lanes.Length);
                SpawnCoin(coinLane);
            }
        }

        private void SpawnPedestrian(int lane)
        {
            var ped = GetFromPool<Pedestrian>(_pedestrianPool);
            if (ped == null) return;

            var laneDef = config.lanes[lane];
            float speed = config.baseScrollSpeed * _currentSpeedMultiplier;
            speed *= laneDef.isOncoming
                ? config.oncomingSpeedMultiplier
                : config.sameDirectionSpeedMultiplier;

            ped.transform.position = new Vector3(
                laneDef.xPosition, config.spawnY, 0f);
            ped.Activate(speed, config.despawnY, lane);

            ped.OnPassedPlayer -= HandlePedestrianPassed;
            ped.OnPassedPlayer += HandlePedestrianPassed;
            ped.OnDespawned -= HandleDespawn;
            ped.OnDespawned += HandleDespawn;
        }

        private void SpawnIce(int lane)
        {
            var ice = GetFromPool<IcePatch>(_icePool);
            if (ice == null) return;
            var laneDef = config.lanes[lane];
            float speed = config.baseScrollSpeed * _currentSpeedMultiplier;
            ice.transform.position = new Vector3(
                laneDef.xPosition, config.spawnY, 0f);
            ice.Activate(speed, config.despawnY, lane);
            ice.OnDespawned -= HandleDespawn;
            ice.OnDespawned += HandleDespawn;
        }

        private void SpawnCoin(int lane)
        {
            var coin = GetFromPool<Coin>(_coinPool);
            if (coin == null) return;
            var laneDef = config.lanes[lane];
            float speed = config.baseScrollSpeed * _currentSpeedMultiplier;
            coin.transform.position = new Vector3(
                laneDef.xPosition, config.spawnY, 0f);
            coin.Activate(speed, config.despawnY, lane);
            coin.OnDespawned -= HandleDespawn;
            coin.OnDespawned += HandleDespawn;
        }

        //============ POOL MANAGEMENT ============
        private void BuildPools()
        {
            _pedestrianPool = CreatePool(pedestrianPrefab, config.pedestrianPoolSize);
            _icePool = CreatePool(icePrefab, config.icePoolSize);
            _coinPool = CreatePool(coinPrefab, config.coinPoolSize);
        }

        private Queue<ObstacleBase> CreatePool(GameObject prefab, int size)
        {
            var pool = new Queue<ObstacleBase>();
            for (int i = 0; i < size; i++)
            {
                var go = Instantiate(prefab, transform);
                go.SetActive(false);
                var obs = go.GetComponent<ObstacleBase>();
                if (obs != null) pool.Enqueue(obs);
            }
            return pool;
        }

        private T GetFromPool<T>(Queue<ObstacleBase> pool) where T : ObstacleBase
        {
            if (pool.Count == 0) return null;
            var item = pool.Dequeue();
            return item as T;
        }

        private void HandleDespawn(ObstacleBase obs)
        {
            obs.OnPassedPlayer -= HandlePedestrianPassed;
            obs.OnDespawned -= HandleDespawn;
            if (obs is Pedestrian) _pedestrianPool.Enqueue(obs);
            else if (obs is IcePatch) _icePool.Enqueue(obs);
            else if (obs is Coin) _coinPool.Enqueue(obs);
        }

        private void HandlePedestrianPassed(ObstacleBase obs)
        {
            if (obs is Pedestrian ped)
                OnPedestrianPassed?.Invoke(ped);
        }


    }
}


