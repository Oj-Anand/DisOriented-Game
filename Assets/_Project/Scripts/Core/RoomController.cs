using UnityEngine;
using TMPro;
using DisOriented.Core.Events;
using DisOriented.Data;
using DisOriented.UI;

namespace DisOriented.Core
{
    /// <summary>Manages camrra, hotspots, minigame returns and HUD visibility</summary>
    public class RoomController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RoomCameraController cameraController;
        [SerializeField] private ResourceHUD resourceHUD;
        [SerializeField] private TextMeshProUGUI returnInfoLabel;

        [Header("Hotspots")]
        [SerializeField] private HotspotInteractable[] hotspots;

        /// <summary>Static field used to pass minigame results</summary>
        public static MinigameResultData LastMinigameResult { get; set; }
        void Start()
        {
            //HUD is visible in room 
            if (resourceHUD != null)
                resourceHUD.ShowAll();

            //Check for minigame return
            HandleMinigameReturn();

        }

        private void HandleMinigameReturn()
        {
            var result = LastMinigameResult;
            if (result == null) return;

            if (returnInfoLabel != null)
            {
                string outcome = result.Succeeded ? "Survived!" : "Wiped out!";
                returnInfoLabel.text =
                    $"{result.MinigameName}: {outcome}\n" +
                    $"Aura Points: {result.ScorePercentage * 1000:F0}\n" +
                    $"Cash: ${result.CashCollected:F0}";
                returnInfoLabel.gameObject.SetActive(true);
            }

            LastMinigameResult = null;
        }

    }
}

