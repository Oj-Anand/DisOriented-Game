using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DisOriented.Data;

namespace DisOriented.UI
{
    /// <summary>
    /// Manages the group of 4 resource meter UI elements.
    /// Handles show/hide transitions and selective meter display.
    /// </summary>
    public class ResourceHUD : MonoBehaviour
    {
        [Header("Meter References")]
        [SerializeField] private ResourceMeterUI[] meters;

        [Header("Animation")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.2f;

        private Dictionary<ResourceType, ResourceMeterUI> _meterMap;

        private void Awake()
        {
            _meterMap = new Dictionary<ResourceType, ResourceMeterUI>();
            foreach (var meter in meters)
            {
                if (meter != null)
                    _meterMap[meter.Type] = meter;
            }

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
        }

        // ---- VISIBILITY API ----

        /// <summary>Show all meters with fade-in.</summary>
        public void ShowAll()
        {
            foreach (var meter in meters)
                meter.gameObject.SetActive(true);
            FadeIn();
        }

        /// <summary>Hide all meters with fade-out.</summary>
        public void HideAll()
        {
            FadeOut(() => {
                foreach (var meter in meters)
                    meter.gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// Show only specific resource meters during minigames.
        /// </summary>
        public void ShowOnly(params ResourceType[] typesToShow)
        {
            var showSet = new HashSet<ResourceType>(typesToShow);
            foreach (var kvp in _meterMap)
            {
                kvp.Value.gameObject.SetActive(showSet.Contains(kvp.Key));
            }
            FadeIn();
        }

        /// <summary>Get a specific meter for external access.</summary>
        public ResourceMeterUI GetMeter(ResourceType type)
        {
            return _meterMap.TryGetValue(type, out var meter) ? meter : null;
        }

        // ---- FADE HELPERS ----

        private void FadeIn()
        {
            if (canvasGroup == null) return;
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, fadeInDuration).SetEase(Ease.OutQuad);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        private void FadeOut(System.Action onComplete = null)
        {
            if (canvasGroup == null)
            {
                onComplete?.Invoke();
                return;
            }
            canvasGroup.DOKill();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}
