using System;
using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    public class RunnerScoreManager : MonoBehaviour
    {
        private RunnerConfig _config;
        private int _score;
        private int _cash;
        private int _pedestriansPassed;

        public int Score => _score;
        public int Cash => _cash;
        public int PedestriansPassed => _pedestriansPassed;

        public event Action<int> OnScoreChanged;
        public event Action<int> OnCashChanged;

        public void Initialize(RunnerConfig cfg)
        {
            _config = cfg;
            _score = 0;
            _cash = 0;
            _pedestriansPassed = 0;
        }

        public void AddPedestrianPass(Pedestrian ped)
        {
            var laneDef = _config.lanes[ped.LaneIndex];
            int points = Mathf.RoundToInt(
                _config.pointsPerPedestrianPassed * laneDef.scoreMultiplier);
            _score += points;
            _pedestriansPassed++;
            OnScoreChanged?.Invoke(_score);
        }

        public void AddCash(int amount)
        {
            _cash += amount;
            OnCashChanged?.Invoke(_cash);
        }
    }
}
