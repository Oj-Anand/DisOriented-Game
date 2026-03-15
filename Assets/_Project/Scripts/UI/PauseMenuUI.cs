using DisOriented.Core;
using DisOriented.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace DisOriented.UI
{
    /// <summary>Pause menu controller</summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject pausePanel;

        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartMinigameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button saveQuitButton;

        [Header("Button UIs")]
        [SerializeField] private MenuButtonUI restartMinigameButtonUI;

        [Header("Scene names")]
        [SerializeField] private string mainMenuScene = "MainMenu"; 
        private void Awake()
        {
            pausePanel.SetActive(false);

            resumeButton.onClick.AddListener(OnResume);
            restartMinigameButton.onClick.AddListener(OnRestartMinigame);
            settingsButton.onClick.AddListener(OnSettings);
            saveQuitButton.onClick.AddListener(OnSaveQuit);
        }

        private void OnEnable()
        {
            var gsm = GameStateManager.Instance;
            if (gsm != null)
            {
                gsm.OnPaused += ShowPause;
                gsm.OnResumed += HidePause; 
            }
        }

        private void OnDisable()
        {
            var gsm = GameStateManager.Instance;
            if (gsm != null)
            {
                gsm.OnPaused -= ShowPause;
                gsm.OnResumed -= HidePause;
            }
        }

        private void Update()
        {
            //Listen for pause input 
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                GameStateManager.Instance?.TogglePause();
            }

        }

        private void ShowPause()
        {
            pausePanel.SetActive(true);

            //show restart minigame only if we are in a minigame when we pause
            bool inMiniGame = GameStateManager.Instance.StateBeforePause == GameState.Minigame;
            restartMinigameButton.gameObject.SetActive(inMiniGame); 
        }

        private void HidePause()
        {
            pausePanel.SetActive(false);
        }

        private void OnResume()
        {
            GameStateManager.Instance?.TogglePause();
        }

        private void OnRestartMinigame()
        {
            //unpause and reload the minigame scene
            var gsm = GameStateManager.Instance;
            string currentScene = SceneTransitionManager.Instance.CurrentSceneName;

            gsm.TogglePause(); //resume

            SceneTransitionManager.Instance.LoadScene(currentScene,
                onMidTransition: () =>
                {
                    //stay in minigame scene and reload state
                    ResourceManager.Instance?.ResetAll();
                });
        }

        private void OnSettings()
        {
            Debug.Log("[Pause] Settings [NOT IMPLEMENTED YET]"); 
        }

        private void OnSaveQuit()
        {
            Debug.Log("[Pause] Save and Quit");

            //Unpause then go to menu
            var gsm = GameStateManager.Instance;
            string currentScene = SceneTransitionManager.Instance.CurrentSceneName;

            gsm.TogglePause(); //resume

            SceneTransitionManager.Instance.LoadScene(mainMenuScene,
                onMidTransition: () =>
                {
                    GameStateManager.Instance.TransitionTo(GameState.MainMenu);
                });
        }
    }
}


