using UnityEngine;
using DG.Tweening;
using DisOriented.Data;
using UnityEngine.InputSystem;


namespace DisOriented.Core
{
    /// <summary>
    /// Obritting camera controller for room diorama 
    /// rotates to face each wall with player input
    /// </summary>
    public class RoomCameraController : MonoBehaviour
    {
        [Header("Orbit Settings")]
        [SerializeField] private Transform orbitTarget; //The center of the room 
        [SerializeField] private float orbitDistance = 5f; //distance from da center
        [SerializeField] private float orbitHeight = 2f; //the y offset for the camera 
        [SerializeField] private float rotationDuration = 0.6f;
        [SerializeField] private Ease rotationEase = Ease.InOutQuad;
        [SerializeField] private float pitchAngle = 19f;

        [Header("Wall Angles")]
        [SerializeField] private float[] wallAngles = { 0f, 90f, 180f, 270f };

        //=============== STATE ================
        private int _currentWallIndex = 0;
        private bool _isRotating = false;
        private Tweener _rotateTween;
        private float _currentAngle = 0f;

        //=============== PUBLIC ================
        public RoomWall CurrentWall => (RoomWall)_currentWallIndex;
        public bool IsRotating => _isRotating;

        /// <summary>Fires when camera finishes rotating to a new wall</summary>
        public event System.Action<RoomWall> OnWallChanged;

        private void Start()
        {
            if (orbitTarget == null)
            {
                Debug.LogError("[ROOM CAMERA] orbit target not assigned");
                return;
            }
            //Snap to initial position 
            _currentAngle = wallAngles[_currentWallIndex];
            ApplyCameraPosition(_currentAngle);
        }

        private void Update()
        {
            if (_isRotating) return;

            //Check GameState to prevent rotation during wrong states 
            var gsm = GameStateManager.Instance;
            if (gsm == null || gsm.CurrentState != GameState.Room) return;

            if (Keyboard.current == null) return;

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
            {
                RotateToWall((_currentWallIndex + 1) % 4);
            }
            else if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
            {
                RotateToWall((_currentWallIndex + 3) % 4); //+3 = -1 mod 4
            }
        }

        public void RotateToWall(int wallIndex)
        {
            if (_isRotating || wallIndex == _currentWallIndex) return;
            wallIndex = Mathf.Clamp(wallIndex, 0, 3);

            _isRotating = true; 
            _currentWallIndex = wallIndex;
            float targetAngle = wallAngles[wallIndex];

            //find shortest rotation path 
            float delta = Mathf.DeltaAngle(_currentAngle, targetAngle);
            float endAngle = _currentAngle + delta;

            _rotateTween?.Kill();
            _rotateTween = DOTween.To(
                () => _currentAngle,
                angle => { _currentAngle = angle; ApplyCameraPosition(angle); },
                endAngle,
                rotationDuration).SetEase(rotationEase).OnComplete(() =>
                {
                    _currentAngle = targetAngle; //Normalize 
                    _isRotating = false;
                    OnWallChanged?.Invoke(CurrentWall);
                }); 
        }

        private void ApplyCameraPosition(float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Sin(rad) * orbitDistance,
                orbitHeight,
                Mathf.Cos(rad) * orbitDistance);
            transform.position = orbitTarget.position + offset;
            transform.LookAt(orbitTarget.position + Vector3.up * (orbitHeight * 0.5f));

            //slight donward tilt 
            Vector3 euler = transform.eulerAngles;
            euler.x = pitchAngle;
            transform.eulerAngles = euler;
        }

        ///<summary>Snap directly to a wall without animation</summary>
        public void SnapToWall(RoomWall wall)
        {
            _currentWallIndex = (int)wall;
            _currentAngle = wallAngles[_currentWallIndex];
            ApplyCameraPosition(_currentAngle); 
        }



    }

}
