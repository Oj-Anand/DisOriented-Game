using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using DisOriented.Data; 
namespace DisOriented.Core
{
    ///<summary>
    ///Boot scene entry point. Inits managers, then loads the main menu scene
    ///Non persistent
    ///</summary>
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private string firstSceneToLoad = "MainMenu";

        private IEnumerator Start()
        {
            Debug.Log("[BOOTLOADER] Initializing....");
            
            //Singletons auto initilaize 
            //wait one frame to ensure all awakes fire 
            yield return null;

            //Validate all required managers exist 
            bool ok = true;
            ok &= ValidateManager(ResourceManager.Instance, "ResourceManager");
            ok &= ValidateManager(SaveManager.Instance, "SaveManager");
            ok &= ValidateManager(TimeManager.Instance, "TimeManager");
            ok &= ValidateManager(GameStateManager.Instance, "GameStateManager");
            ok &= ValidateManager(SceneTransitionManager.Instance, "SceneTransitionManager");

            if (!ok)
            {
                Debug.LogError("[BOOT] Missing managers ! Cannot continue...");
                yield break;
            }

            Debug.Log("[BOOT] All managers OK! Loading menu...");

            //Transition to main menu 
            GameStateManager.Instance.TransitionTo(GameState.MainMenu);
            SceneTransitionManager.Instance.LoadScene(firstSceneToLoad);

        }

        private bool ValidateManager<T>(T instance, string name) where T : class
        {
            if (instance == null)
            {
                Debug.LogError($"[BOOT] {name} is missing !");
                return false; 
            }

            return true; 
        }

    }
}
