using UnityEngine;

namespace DisOriented.Core
{
    /// <summary>
    /// Persistent singleton base class for all manager Monobehaviors
    /// Destorys duplicates
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationQuitting = false;
         
        public static T Instance
        {
            get
            {
                if (_applicationQuitting)
                {
                    Debug.LogWarning($"[Singleton] Instance of {typeof(T)}" + "requested after application quit. Returning null.");
                    return null;
                }
                
                //protecting against a potential race condition since we plan on using async loading
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindAnyObjectByType<T>();

                        if (_instance == null)
                        {
                            Debug.LogError($"[Singleton] No instance of" + $"{typeof(T)} found in scene." + "Ensure it exists on the [Managers] Gameobject"); 
                        }
                    }
                    return _instance;
                }
            }

            
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate {typeof(T)} destoryed.");
                Destroy(gameObject);
                return; 
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
            
        }

        protected void OnApplicationQuit()
        {
            _applicationQuitting = true;
        }
    }
}
