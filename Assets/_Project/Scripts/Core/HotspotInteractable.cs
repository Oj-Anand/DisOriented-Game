using UnityEngine;
using DisOriented.Data;
using UnityEngine.InputSystem;

namespace DisOriented.Core
{
    ///<summary>Clickable room objhect that launches a minigame</summary>
    [RequireComponent(typeof(Collider))]
    public class HotspotInteractable : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private RoomWall associatedWall;
        [SerializeField] private string targetScene;
        [SerializeField] private string tooltipText = "Play";

        [Header("Resource cost to play")]
        [SerializeField] private ResourceType costResource = ResourceType.Energy;
        [SerializeField] private float costAmount = 5f;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject highlightEffect; //glow outline 
        [SerializeField] private float hoverScaleMultiplier = 1.05f;
        private Vector3 _originalScale;
        private bool _isHovered;
        private Camera _mainCam;

        public RoomWall Wall => associatedWall; 
        public string TargetScene => targetScene;
        void Start()
        {
            _originalScale = transform.localScale;
            _mainCam = Camera.main;
            if(highlightEffect != null) highlightEffect.SetActive(false);
        }
        void Update()
        {
            var gsm = GameStateManager.Instance;
            if (gsm == null || gsm.CurrentState != GameState.Room) return; 

            //Raycast from mouse to detect hover
            Ray ray = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            bool hitting = Physics.Raycast(ray, out RaycastHit hit, 100f) && hit.collider.gameObject == gameObject;

            if (hitting && !_isHovered) OnHoverEnter();
            else if (!hitting && _isHovered) OnHoverExit();

            //click detection
            if (hitting && Mouse.current.leftButton.wasPressedThisFrame)
                OnClicked();
        }

        private void OnHoverEnter()
        {
            _isHovered = true;
            transform.localScale = _originalScale * hoverScaleMultiplier;
            if (highlightEffect != null) highlightEffect.SetActive(true);
        }

        private void OnHoverExit()
        {
            _isHovered = false;
            transform.localScale = _originalScale ;
            if (highlightEffect != null) highlightEffect.SetActive(false);
        }

        private void OnClicked()
        {
            //verify camera is facing the wall the object is along
            var cam = FindFirstObjectByType<RoomCameraController>();
            if (cam != null && cam.CurrentWall != associatedWall)
            {
                Debug.Log($"[HOTSPOT] Not facing {associatedWall}, click ignored");
                
                return; 
            }

            //Check if player can afford it 
            var rm = ResourceManager.Instance;
            if (rm != null && rm.GetValue(costResource) < costAmount)
            {
                Debug.Log($"[HOTSPOT] Not enough {costResource}");
                //TODO SHOW UI FEEDBACK
                return;
            }

            //deduct cost and launch 
            rm?.Modify(costResource, -costAmount);

            Debug.Log($"[HOTSPOT] Launching {targetScene}");

            var gsm = GameStateManager.Instance;
            SceneTransitionManager.Instance.LoadScene(targetScene,
                onMidTransition: () =>
                {
                    gsm.TransitionTo(GameState.Minigame);
                }
            );

        }


    }
}


