using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DisOriented.Core;
using DisOriented.Core.Events;
using DisOriented.Data;

namespace DisOriented.UI
{
    /// <summary>
    /// Pulsing glow effect for critically low resources.
    /// Attach to the same GameObject as ResourceMeterUI.
    /// Requires a child Image named "PulseGlow".
    /// </summary>

    [RequireComponent(typeof(ResourceMeterUI))]
    public class CriticalPulse : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image pulseGlowImage;

        [Header("Pulse Settings")]
        [SerializeField] private float pulseMinAlpha = 0.0f;
        [SerializeField] private float pulseMaxAlpha = 0.6f;
        [SerializeField] private float pulseDuration = 0.8f;
        [SerializeField] private Color pulseColor = new Color(1f, 0.3f, 0.2f, 1f);

        private ResourceMeterUI _meter;
        private Sequence _pulseSequence;
        private bool _isPulsing;

        private void Awake()
        {
            _meter = GetComponent<ResourceMeterUI>();
            if (pulseGlowImage != null)
            {
                pulseGlowImage.color = new Color(
                    pulseColor.r, pulseColor.g, pulseColor.b, 0f);
                pulseGlowImage.enabled = false;
            }
        }

        private void OnEnable()
        {
            var rm = ResourceManager.Instance;
            if (rm != null)
            {
                rm.OnResourceCritical += HandleCritical;
                rm.OnResourceRecovered += HandleRecovered;

                // Check if already critical on enable
                if (rm.IsCritical(_meter.Type))
                    StartPulse();
            }
        }

        private void OnDisable()
        {
            var rm = ResourceManager.Instance;
            if (rm != null)
            {
                rm.OnResourceCritical -= HandleCritical;
                rm.OnResourceRecovered -= HandleRecovered;
            }
            StopPulse();
        }

        private void HandleCritical(ResourceCriticalEvent e)
        {
            if (e.Type == _meter.Type)
                StartPulse();
        }

        private void HandleRecovered(ResourceType type)
        {
            if (type == _meter.Type)
                StopPulse();
        }

        private void StartPulse()
        {
            if (_isPulsing) return;
            _isPulsing = true;

            if (pulseGlowImage == null) return;
            pulseGlowImage.enabled = true;

            _pulseSequence?.Kill();
            _pulseSequence = DOTween.Sequence()
                .Append(pulseGlowImage.DOFade(pulseMaxAlpha, pulseDuration))
                .Append(pulseGlowImage.DOFade(pulseMinAlpha, pulseDuration))
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.InOutSine);
        }

        private void StopPulse()
        {
            _isPulsing = false;
            _pulseSequence?.Kill();

            if (pulseGlowImage != null)
            {
                pulseGlowImage.DOFade(0f, 0.3f)
                    .OnComplete(() => pulseGlowImage.enabled = false);
            }
        }

    }
}


