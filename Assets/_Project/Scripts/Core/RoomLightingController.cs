using UnityEngine;
using DG.Tweening;
using DisOriented.Core.Events;
using DisOriented.Data;

namespace DisOriented.Core
{
    /// <summary>Adjusts room lighting based on time of day </summary>
    public class RoomLightingController : MonoBehaviour
    {
        [Header("Light References")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private Light ambientFillLight;

        [Header("Lighting Presets ")]
        [Tooltip("(index = TimePhase enum)")]
        [SerializeField]
        private LightingPreset[] presets = new LightingPreset[]
        {
            new LightingPreset { color = new Color(1f, 0.95f, 0.8f), intensity = 1.2f,
                ambientColor = new Color(1f, 0.97f, 0.9f) },  // Morning
            new LightingPreset { color = new Color(1f, 1f, 1f), intensity = 1.0f,
                ambientColor = new Color(0.95f, 0.95f, 0.95f) },  // Afternoon
            new LightingPreset { color = new Color(1f, 0.75f, 0.5f), intensity = 0.7f,
                ambientColor = new Color(0.8f, 0.7f, 0.5f) },  // Evening
            new LightingPreset { color = new Color(0.5f, 0.55f, 0.8f), intensity = 0.3f,
                ambientColor = new Color(0.3f, 0.3f, 0.5f) },  // Night
        };

        [Header("Transition")]
        [SerializeField] private float transitionDuration = 1f;

        private void OnEnable()
        {
            var tm = TimeManager.Instance;
            if (tm != null)
            {
                tm.OnTimeAdvanced += HandleTimeAdvanced;
                ApplyPreset((int)tm.CurrentPhase, instant: true);
            }
        }

        private void OnDisable()
        {
            var tm = TimeManager.Instance;
            if (tm != null)
                tm.OnTimeAdvanced -= HandleTimeAdvanced;
        }

        private void HandleTimeAdvanced(TimeAdvancedEvent e)
        {
            ApplyPreset((int)e.NewPhase, instant: false);
        }

        private void ApplyPreset(int index, bool instant)
        {
            if (index < 0 || index >= presets.Length) return;

            var p = presets[index];
            float dur = instant ? 0f : transitionDuration;

            if (directionalLight != null)
            {
                directionalLight.DOColor(p.color, dur);
                directionalLight.DOIntensity(p.intensity, dur);
            }

            if (ambientFillLight != null)
                ambientFillLight.DOColor(p.ambientColor, dur);

            RenderSettings.ambientLight = instant ? p.ambientColor: RenderSettings.ambientLight;
            if (!instant)
            {
                DOTween.To(() => RenderSettings.ambientLight,
                    c => RenderSettings.ambientLight = c, p.ambientColor, dur);

            }
        }


        [System.Serializable]
        public class LightingPreset
        {
            public Color color = Color.white;
            public float intensity = 1f;
            public Color ambientColor = Color.gray;
        }

    }
}

