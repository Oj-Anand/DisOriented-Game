using UnityEngine;
using UnityEngine.UI;
using DisOriented.Core;
using DisOriented.Data;

namespace DisOriented.UI
{
    /// <summary>Main Menu Controller</summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [Header("Button UI Components")]
        [SerializeField] private MenuButtonUI continueButtonUI;

        [Header("Scene Names")]
        [SerializeField] private string roomScene = "Room";

        private void Start()
        {
            //Wire button listeners 
            newGameButton.onClick.AddListener(OnNewGame);
            continueButton.onClick.AddListener(OnContinue);
            settingsButton.onClick.AddListener(OnSettings);
            creditsButton.onClick.AddListener(OnCredits);
            quitButton.onClick.AddListener(OnQuit);

            //Configure continue button based on wether save exists 
            bool hasSave = SaveManager.Instance != null && SaveManager.Instance.SaveExists();
            continueButton.interactable = hasSave;
            if (continueButtonUI != null) continueButtonUI.Configure("Continue", hasSave); 

        }

        private void OnNewGame()
        {
            Debug.Log("[MainMenu] New Game");

            //Delete old save 
            SaveManager.Instance?.DeleteSave();
            ResourceManager.Instance?.ResetAll();
            TimeManager.Instance?.ResetToStart();

            //Transition from main menu to room 
            SceneTransitionManager.Instance.LoadScene(roomScene,
                onMidTransition: () =>
                {
                    GameStateManager.Instance.TransitionTo(GameState.Room);
                }); 
        }

        private void OnContinue()
        {
            Debug.Log("[MainMenu] Continue");

            bool loaded = SaveManager.Instance.Load();
            if (!loaded)
            {
                Debug.LogError("[MainMenu] Failed to load save!");
                return; 
            }

            SceneTransitionManager.Instance.LoadScene(roomScene,
                onMidTransition: () =>
                {
                    GameStateManager.Instance.TransitionTo(GameState.Room);
                });
        }

        private void OnSettings()
        {
            //Stub
            Debug.Log("[MainMenu] Settings [NOT IMPLEMENTED YET]"); 
        }

        private void OnCredits()
        {
            //Stub 
            Debug.Log("[MainMenu] Credits [NOT IMPLEMENTED YET]");
        }
        private void OnQuit()
        {
            Debug.Log("[MainMenu] Quit");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit(); 
            #endif
        }

    }
}


