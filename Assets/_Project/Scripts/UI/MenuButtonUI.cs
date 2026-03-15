using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

///<summary>Hower glow + scale effect for menu buttons</summary>
namespace DisOriented.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuButtonUI : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, ISelectHandler, IDeselectHandler
    {
        [Header("References")]
        [SerializeField] private Image glowImage;
        [SerializeField] private TextMeshProUGUI label;

        [Header("Hover Settings")]
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float hoverGlowAlpha = 0.3f;
        [SerializeField] private float hoverDuration = 0.15f;

        private Vector3 _originalScale;
        private Button _button;

        private void Awake()
        {
            _originalScale = transform.localScale;
            _button = GetComponent<Button>();
            if (glowImage != null)
            {
                var c = glowImage.color;
                glowImage.color = new Color(c.r, c.g, c.b, 0f);
            }
        }

        public void OnPointerEnter(PointerEventData eventData) => HoverIn();
        public void OnPointerExit(PointerEventData eventData) => HoverOut();
        public void OnSelect(BaseEventData eventData) => HoverIn();
        public void OnDeselect(BaseEventData eventData) => HoverOut();

        private void HoverIn()
        {
            if (!_button.interactable) return;
            transform.DOKill();
            transform.DOScale(_originalScale * hoverScale, hoverDuration).SetEase(Ease.OutBack).SetUpdate(true);

            if (glowImage != null) glowImage.DOFade(hoverGlowAlpha, hoverDuration).SetUpdate(true);
        }

        private void HoverOut()
        {
            transform.DOKill();
            transform.DOScale(_originalScale, hoverDuration).SetEase(Ease.InQuad).SetUpdate(true);

            if (glowImage != null) glowImage.DOFade(0f, hoverDuration).SetUpdate(true);
        }

        ///<summary>Set button text and interactablke state</summary>
        public void Configure(string text, bool interactable = true)
        {
            if (label != null) label.text = text; 
            _button.interactable = interactable;
            if (label != null) label.alpha = interactable ? 1f : 0.4f; 
        }

        //Handles moving to another scene after button click
        private void OnDestroy()
        {
            transform.DOKill();
            if (glowImage != null) glowImage.DOKill();
        }


    }
}

