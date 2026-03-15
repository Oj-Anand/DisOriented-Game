using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace DisOriented.Minigames.Pacemog
{       
    /// <summary>
    /// Player lane swithcing controller 
    /// moves one lane at a time using A/D
    /// </summary>
    public class PlayerLaneController : MonoBehaviour
    {
        [SerializeField] private RunnerConfig config;
        private int _currentLane;
        private bool _isMoving;
        private bool _isSlowed;     // True when affected by ice
        private float _slowTimer;
        private Tweener _moveTween;

        //============= Public ===============
        public int CurrentLane => _currentLane;
        public LaneDefinition CurrentLaneDef => config.lanes[_currentLane];
        public bool IsSlowed => _isSlowed;
        public void Initialize(RunnerConfig cfg)
        {
            config = cfg;
            _currentLane = config.startingLaneIndex;
            SnapToLane(_currentLane);
        }

        private void Update()
        {
            HandleInput();
            UpdateIceEffect();
        }

        private void HandleInput()
        {
            if (_isMoving) return;

            if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
                TryMoveLane(-1);
            else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
                TryMoveLane(+1);
        }

        private void TryMoveLane(int direction)
        {
            int target = _currentLane + direction;
            if (target < 0 || target >= config.lanes.Length) return;

            _currentLane = target;
            float targetX = config.lanes[target].xPosition;

            float duration = config.laneSwitchDuration;
            if (_isSlowed) duration *= config.iceSlowMultiplier;

            _isMoving = true;
            _moveTween?.Kill();
            _moveTween = transform.DOMoveX(targetX, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _isMoving = false);
        }


        /// <summary>Apply ice slow effect.</summary>
        public void ApplyIceSlow()
        {
            _isSlowed = true;
            _slowTimer = config.iceEffectDuration;
        }
        private void UpdateIceEffect()
        {
            if (!_isSlowed) return;
            _slowTimer -= Time.deltaTime;
            if (_slowTimer <= 0f) _isSlowed = false;
        }

        private void SnapToLane(int lane)
        {
            var pos = transform.position;
            pos.x = config.lanes[lane].xPosition;
            transform.position = pos;
        }

    }
}

