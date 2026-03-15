using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace DisOriented.Core
{
    /// <summary>
    /// Handles all scene loading with fade to black transitions
    /// </summary>
    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        [Header("Transition Settings")]
        [SerializeField] private float fadeDuration = 0.4f;
        [SerializeField] private Color fadeColor = Color.black;

        //=========== INTERNAL ============
        private CanvasGroup _fadeOverlay;
        private Canvas _fadeCanvas;
        private bool _isTransitioning; 

        //========== PUBLIC ==============
        public bool IsTransitioning => _isTransitioning;
        public string CurrentSceneName =>
    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        //#################################
        //          INITIALIZATION
        //#################################

        protected override void OnInitialize()
        {
            CreateFadeOverlay(); 
        }

        ///<summary>Build the fade overlay</summary>
        private void CreateFadeOverlay()
        {
            //Create canvas 
            var fadeGO = new GameObject("[FadeOverlay]");
            fadeGO.transform.SetParent(transform); 
            _fadeCanvas = fadeGO.AddComponent<Canvas>();
            _fadeCanvas.sortingOrder = 999;
            _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            fadeGO.AddComponent<UnityEngine.UI.CanvasScaler>();

            //Create full screen image
            var imgGO = new GameObject("[FadeImage]"); 
            imgGO.transform.SetParent(transform, false); 
            var img = imgGO.AddComponent<UnityEngine.UI.Image>();
            img.color = fadeColor;
            img.raycastTarget = true;

            var rt = imgGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;

            //Canvas group for scaling
            _fadeOverlay = fadeGO.AddComponent<CanvasGroup>();
            _fadeOverlay.alpha = 0f;
            _fadeOverlay.blocksRaycasts = false;
            _fadeOverlay.interactable = false; 
        }


        //#########################
        //       PUBLIC API
        //#########################

        ///<summary>Load a scene with fade out - load - fade in transition</summary>
        public void LoadScene(string sceneName, Action onMidTransition = null, Action onComplete = null)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("[SCENETRANSITIONMANAGER] Transition already in progress");
                return;
            }

            StartCoroutine(TransitionCoroutine(sceneName, onMidTransition, onComplete));
        }

        ///<summary>Fade to black, execute action, fade back in - doesnt load new scene </summary>
        public void FadeAction(Action midAction, Action onComplete = null)
        {
            if (_isTransitioning) return;
            StartCoroutine(FadeActionCoroutine(midAction, onComplete));
        }

        //########################
        //       COROUTINES
        //########################

        private IEnumerator TransitionCoroutine(string sceneName, Action onMidTransition, Action onComplete)
        {
            _isTransitioning = true;

            //Fade out 
            yield return FadeOut();

            //mid transiton callback 
            onMidTransition?.Invoke();

            //load scene 
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName);

            if (loadOp == null)
            {
                Debug.LogError($"[SCENETRANSITIONMANAGER] Failed to load '{sceneName}'");

                yield return FadeIn();
                _isTransitioning = false; 
                yield break;
            }

            while (!loadOp.isDone) yield return null;

            //wait a frame for awake and start methods to finish
            yield return null;

            //Fade in from black 
            yield return FadeIn();

            _isTransitioning = false; 
            onComplete?.Invoke();
        }

        private IEnumerator FadeActionCoroutine(Action midAction, Action onComplete)
        {
            _isTransitioning = true;
            yield return FadeOut();
            midAction?.Invoke(); 
            yield return FadeIn();
            _isTransitioning = false;
            onComplete?.Invoke(); 
        }

        private IEnumerator FadeOut()
        {
            _fadeOverlay.blocksRaycasts = true;
            _fadeOverlay.DOKill();
            _fadeOverlay.DOFade(1f, fadeDuration).SetUpdate(true); //ignore timescale so it works in pause
            yield return new WaitForSecondsRealtime(fadeDuration);

        }

        private IEnumerator FadeIn()
        {
            _fadeOverlay.DOKill();
            _fadeOverlay.DOFade(0f, fadeDuration).SetUpdate(true); //ignore timescale so it works in pause
            yield return new WaitForSecondsRealtime(fadeDuration);
            _fadeOverlay.blocksRaycasts = false;

        }
    }
}
