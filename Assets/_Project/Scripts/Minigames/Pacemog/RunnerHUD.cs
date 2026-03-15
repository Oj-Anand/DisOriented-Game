using UnityEngine;
using TMPro;
using DG.Tweening;

namespace DisOriented.Minigames.Pacemog
{
    public class RunnerHUD : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI timerLabel;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private TextMeshProUGUI cashLabel;
        [SerializeField] private GameObject iceWarningIcon;

        private RunnerScoreManager _scoreManager;

        public void Initialize(RunnerScoreManager scoreMgr)
        {
            _scoreManager = scoreMgr;
            _scoreManager.OnScoreChanged += UpdateScore;
            _scoreManager.OnCashChanged += UpdateCash;
            UpdateScore(0);
            UpdateCash(0);
            if (iceWarningIcon != null) iceWarningIcon.SetActive(false);
        }

        public void UpdateTimer(float secondsRemaining)
        {
            if (timerLabel == null) return;
            int s = Mathf.CeilToInt(Mathf.Max(0f, secondsRemaining));
            timerLabel.text = $"{s / 60}:{s % 60:D2}";
            //flash warning in final 10 seconds
            if (s <= 10 && s > 0)
                timerLabel.color = Color.Lerp(Color.red, Color.white,
                    Mathf.PingPong(Time.time * 3f, 1f));
        }

        private void UpdateScore(int score)
        {
            if (scoreLabel != null)
            {
                scoreLabel.text = $"Aura: {score}";
                scoreLabel.transform.DOKill();
                scoreLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 4);
            }
        }

        private void UpdateCash(int cash)
        {
            if (cashLabel != null) cashLabel.text = $"${cash}";
        }

        public void ShowIceWarning(bool show)
        {
            if (iceWarningIcon != null) iceWarningIcon.SetActive(show);
        }

        private void OnDestroy()
        {
            if (_scoreManager != null)
            {
                _scoreManager.OnScoreChanged -= UpdateScore;
                _scoreManager.OnCashChanged -= UpdateCash;
            }
        }
    }
}
