using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

            //Validate critical managers exist 
            if(ResourceManager.Instance == null)
            {
                Debug.LogError("[BootLoader] ResourceManager missing!");
                yield break;
            }
            if (SaveManager.Instance == null)
            {
                Debug.LogError("[BootLoader] SaveManager missing!");
                yield break;
            }

            Debug.Log("[BOOTLOADER] All critical manager initialized. Loading Menu");
            SceneManager.LoadScene(firstSceneToLoad); 


        }

    }
}
