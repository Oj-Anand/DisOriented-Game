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
    /// Displays current day and time phase in the HUD.
    /// Subscribes to TimeManager events for reactive updates.
    /// </summary>

    public class TimeDisplayUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI dayLabel;
        [SerializeField] private TextMeshProUGUI phaseLabel;
        [SerializeField] private Image calendarIcon;
        [SerializeField] private Image phaseTintBar;

        [Header("Animation")]
        [SerializeField] private float transitionDuration = 0.4f;

        //#########################
        //        LIFECYCLE
        //#########################
        private void OnEnable()
        {
            var tm = TimeManager.Instance;
            if (tm != null)
            {
                tm.OnTimeAdvanced += HandleTimeAdvanced;
                tm.OnDayChanged += HandleDayChanged;

                // Initialize to current state
                RefreshDisplay(tm);
            }
        }

        private void OnDisable()
        {
            var tm = TimeManager.Instance;
            if (tm != null)
            {
                tm.OnTimeAdvanced -= HandleTimeAdvanced;
                tm.OnDayChanged -= HandleDayChanged;
            }
        }

        //#########################
        //     EVENT HANDLERS
        //#########################
        private void HandleTimeAdvanced(TimeAdvancedEvent e)
        {
            var tm = TimeManager.Instance;
            if (tm == null) return;

            AnimatePhaseTransition(tm);
        }

        private void HandleDayChanged(DayChangedEvent e)
        {
            // Day label update with a punch scale for emphasis
            if (dayLabel != null)
            {
                dayLabel.text = $"Day {e.NewDay} - {e.DayName}";
                dayLabel.transform.DOKill();
                dayLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 4, 0.5f);
            }
        }

        //#########################
        //     DISPLAY LOGIC
        //#########################

        /// <summary>Refresh all display elements to current state.</summary>
        private void RefreshDisplay(TimeManager tm)
        {
            if (dayLabel != null) dayLabel.text = tm.DayDisplayString;

            if (phaseLabel != null) phaseLabel.text = tm.PhaseDisplayString;

            UpdatePhaseTint(tm.CurrentPhase, tm.Definition);
        }

        /// <summary>Animate the phase label and tint bar transition.</summary>
        private void AnimatePhaseTransition(TimeManager tm)
        {
            // Fade out old phase text, swap, fade in
            if (phaseLabel != null)
            {
                phaseLabel.DOKill();
                phaseLabel.DOFade(0f, transitionDuration * 0.4f)
                    .OnComplete(() =>
                    {
                        phaseLabel.text = tm.PhaseDisplayString;
                        phaseLabel.DOFade(1f, transitionDuration * 0.6f);
                    });
            }

            if (dayLabel != null)
                dayLabel.text = tm.DayDisplayString;

            UpdatePhaseTint(tm.CurrentPhase, tm.Definition);
        }

        /// <summary>Tint the phase bar to the current phase's colour.</summary>
        private void UpdatePhaseTint(TimePhase phase, TimeDefinition def)
        {
            if (phaseTintBar == null || def == null) return;

            Color targetColor = def.GetPhaseConfig(phase).GetColor();
            phaseTintBar.DOKill();
            phaseTintBar.DOColor(targetColor, transitionDuration).SetEase(Ease.InOutQuad);
        }

    }
}

