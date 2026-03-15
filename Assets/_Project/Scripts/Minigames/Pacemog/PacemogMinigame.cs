using UnityEngine;
using DisOriented.Core;
using DisOriented.Data;

namespace DisOriented.Minigames.Pacemog
{
    public class PacemogMinigame : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private RunnerConfig config;

        [Header("System References")]
        [SerializeField] private PlayerLaneController player;
        [SerializeField] private RunnerCollisionHandler collisionHandler;
        [SerializeField] private ObstacleSpawner spawner;
        [SerializeField] private RoadScroller roadScroller;
        [SerializeField] private DifficultyManager difficultyManager;
        [SerializeField] private RunnerScoreManager scoreManager;
        [SerializeField] private RunnerHUD hud;

        [Header("Scenes")]
        [SerializeField] private string roomScene = "Room";

        private float _timeRemaining;
        private bool _isRunning;
        private bool _isGameOver;

        private void Start()
        {
            InitializeAllSystems();
            StartRun();
        }

        private void InitializeAllSystems()
        {
            player.Initialize(config);
            spawner.Initialize(config);
            scoreManager.Initialize(config);
            difficultyManager.Initialize(config, spawner, roadScroller);
            hud.Initialize(scoreManager);

            //wire collision events
            collisionHandler.OnHitPedestrian += OnGameOver;
            collisionHandler.OnCollectCoin += OnCoinCollected;
            collisionHandler.OnHitIce += OnIceHit;

            //wire pedestrian passed events
            spawner.OnPedestrianPassed += scoreManager.AddPedestrianPass;
        }

        private void StartRun()
        {
            _timeRemaining = config.runDurationSeconds;
            _isRunning = true;
            _isGameOver = false;
        }

        private void Update()
        {
            if (!_isRunning) return;

            _timeRemaining -= Time.deltaTime;
            hud.UpdateTimer(_timeRemaining);

            // Ice warning sync
            hud.ShowIceWarning(player.IsSlowed);

            if (_timeRemaining <= 0f)
                OnTimerComplete();
        }

        //============ EVENTS ==========
        private void OnGameOver()
        {
            if (_isGameOver) return;
            _isGameOver = true;
            _isRunning = false;
            spawner.StopSpawning();

            Debug.Log("[Pacemog] GAME OVER!");
            EndRun(succeeded: false);
        }

        private void OnTimerComplete()
        {
            _isRunning = false;
            spawner.StopSpawning();

            Debug.Log("[Pacemog] Timer complete! You survived!");
            EndRun(succeeded: true);
        }

        private void OnCoinCollected(int value)
        {
            scoreManager.AddCash(value);
        }

        private void OnIceHit()
        {
            player.ApplyIceSlow();
        }

        //========== END RUN =============
        private void EndRun(bool succeeded)
        {
            var result = BuildResult(succeeded);
            result.ApplyRewards();
            TimeManager.Instance?.AdvanceTime();

            RoomController.LastMinigameResult = result;

            //brief delay before transition to let playrs see the result
            Invoke(nameof(TransitionToResults), 1.5f);
        }

        private MinigameResultData BuildResult(bool succeeded)
        {
            var result = new MinigameResultData
            {
                MinigameName = "Pacemog",
                Succeeded = succeeded,
                ScorePercentage = scoreManager.Score / 10000f,
                StarsEarned = CalculateStars(scoreManager.Score),
                CashCollected = scoreManager.Cash,
            };

            if (succeeded)
            {
                float bonusMood = (scoreManager.Score / 1000f)
                    * config.bonusMoodPerThousandPoints;
                float bonusSwag = (scoreManager.Score / 1000f)
                    * config.bonusSwagPerThousandPoints;
                result.ResourceDeltas[ResourceType.Mood] =
                    config.successMoodReward + bonusMood;
                result.ResourceDeltas[ResourceType.Swag] =
                    config.successSwagReward + bonusSwag;
            }
            else
            {
                result.ResourceDeltas[ResourceType.Energy] =
                    config.gameOverEnergyPenalty;
                result.ResourceDeltas[ResourceType.Tummy] =
                    config.gameOverTummyPenalty;
            }

            return result;
        }

        private int CalculateStars(int score)
        {
            if (score >= 5000) return 3;
            if (score >= 2000) return 2;
            if (score >= 500) return 1;
            return 0;
        }

        private void TransitionToResults()
        {
            var gsm = GameStateManager.Instance;
            gsm.TransitionTo(GameState.MinigameResults);

            SceneTransitionManager.Instance.LoadScene(roomScene,
                onMidTransition: () =>
                {
                    gsm.TransitionTo(GameState.Room);
                }
            );
        }

        private void OnDestroy()
        {
            if (collisionHandler != null)
            {
                collisionHandler.OnHitPedestrian -= OnGameOver;
                collisionHandler.OnCollectCoin -= OnCoinCollected;
                collisionHandler.OnHitIce -= OnIceHit;
            }



        }
    }
}


