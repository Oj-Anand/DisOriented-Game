using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DisOriented.Core;
using DisOriented.Core.Events;
using DisOriented.Data;

namespace DisOriented.UI
{
    /// <summary>
    /// Single resource meter bar. 
    /// Subscribes to ResourceManager events and animates fill.
    /// </summary>
    public class ResourceMeterUI : MonoBehaviour
    {
        // ---- Configuration ----

        [Header("Resource Type")]
        [SerializeField] private ResourceType resourceType;

        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Image barFill;
        [SerializeField] private Image patternOverlay;
        [SerializeField] private TextMeshProUGUI valueLabel;

        [Header("Fill Colours")]
        [SerializeField] private Color fillColorStart = Color.white;
        [SerializeField] private Color fillColorEnd = Color.white;

        [Header("Animation Settings")]
        [SerializeField] private float fillAnimDuration = 0.4f;
        [SerializeField] private Ease fillEase = Ease.OutQuad;

        // ---- Internal ----
        private Tweener _fillTween;
        private float _displayedFill; // Currently displayed fill (0-1)

        // ---- Public Access ----
        public ResourceType Type => resourceType;

        //###############################
        //         LIFECYCLE    
        //###############################
        private void OnEnable()
        {
            var rm = ResourceManager.Instance;
            if (rm != null)
            {
                rm.OnResourceChanged += HandleResourceChanged;

                // Initialize to current value (no animation)
                float normalized = rm.GetNormalized(resourceType);
                SetFillImmediate(normalized);
            }
        }

        private void OnDisable()
        {
            var rm = ResourceManager.Instance;
            if (rm != null)
                rm.OnResourceChanged -= HandleResourceChanged;

            _fillTween?.Kill();
        }

        //###############################
        //         EVENT HANDLER    
        //###############################
        private void HandleResourceChanged(ResourceChangeEvent e)
        {
            // Only respond to our resource type
            if (e.Type != resourceType) return;

            AnimateFillTo(e.NormalizedValue);
            UpdateLabel(e.NewValue, e.NormalizedValue);
            UpdateFillColor(e.NormalizedValue);
        }

        //###############################
        //        FILL ANIMATION    
        //###############################

        /// <summary>Animate bar fill to target (0-1).</summary>
        private void AnimateFillTo(float target)
        {
            _fillTween?.Kill();

            _fillTween = DOTween.To(() => _displayedFill,
            x =>{
                    _displayedFill = x;
                    if (barFill != null)
                        barFill.fillAmount = x;
                },
                target,
                fillAnimDuration
            ).SetEase(fillEase);
        }

        /// <summary>Set fill immediately (no animation). For init.</summary>
        private void SetFillImmediate(float normalized)
        {
            _displayedFill = normalized;
            if (barFill != null)
                barFill.fillAmount = normalized;
            UpdateLabel(normalized * 100f, normalized);
            UpdateFillColor(normalized);
        }

        //###############################
        //        VISUAL UPDATES    
        //###############################

        /// <summary>Lerp fill colour between start and end based on fill.</summary>
        private void UpdateFillColor(float normalized)
        {
            if (barFill != null)
                barFill.color = Color.Lerp(fillColorStart, fillColorEnd, normalized);
        }

        /// <summary>Update percentage text label.</summary>
        private void UpdateLabel(float absoluteValue, float normalized)
        {
            if (valueLabel != null)
                valueLabel.text = $"{Mathf.RoundToInt(normalized * 100)}%";
        }

        //###############################
        //        PUBLIC ART SET   
        //###############################

        /// <summary>
        /// Set the resource icon sprite. Called when art assets are loaded or for runtime icon changes.
        /// </summary>
        public void SetIcon(Sprite sprite)
        {
            if (iconImage != null)
            {
                iconImage.sprite = sprite;
                iconImage.enabled = (sprite != null);
            }
        }

    }

}

