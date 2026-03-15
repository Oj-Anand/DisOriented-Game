using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    /// <summary>A pedestrian obstacle</summary>
    public class Pedestrian : ObstacleBase
    {
        private float _playerY;
        private bool _hasPassed;
        private int _laneIndex;

        public int LaneIndex => _laneIndex;

        public override void Activate(float speed, float despawnBound, int laneIndex)
        {
            base.Activate(speed, despawnBound, laneIndex);
            _laneIndex = laneIndex;
            _hasPassed = false;
            _playerY = -3f; //approximate player Y position
        }

        protected override void Update()
        {
            base.Update();
            if (!isActive) return;

            //Check if passed player
            if (!_hasPassed && transform.position.y < _playerY)
            {
                _hasPassed = true;
                NotifyPassed();
            }
        }

    }
}

