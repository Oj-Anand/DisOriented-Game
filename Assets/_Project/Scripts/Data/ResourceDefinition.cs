using UnityEngine;

namespace DisOriented.Data
{
    /// <summary>
    /// SO defiing a single resource's properties 
    /// Each resource (Mood, Swag, Energy, Tummy) gets an instance
    /// </summary>
    [CreateAssetMenu(fileName = "NewResourceDefinition", menuName = "DisOriented/ResourceDefinition")]
    public class ResourceDefinition : ScriptableObject
    {
        [Header("Identity")]
        public ResourceType resourceType;
        public string displayName;

        [Header("Value Bounds")]
        [Tooltip("Minimum possible value ~ 0 ")]
        public float minValue = 0f;

        [Tooltip("Maximum possible value ~ 100 ")]
        public float maxValue = 100f;

        [Tooltip("Starting value")]
        public float startingValue = 50f;

        [Header("Thresholds")]
        [Tooltip("Below this percentage (0-1) trigger critical warning")]
        [Range(0f, 1f)]
        public float criticalThreshold = 0.2f;

        [Tooltip("Above this percentage trigger overflow state")]
        [Range(0f, 1f)]
        public float highThreshold = 0.9f;

        [Header("Passive drain per time phase")]
        [Tooltip("Amount this resource decreases with each phase of the day")]
        public float passiveDrainPerPhase = 0f;

        /// <summary>
        /// Returns the critical threshold as an absolute value.
        /// </summary>
        public float CriticalValue => minValue + (maxValue - minValue) * criticalThreshold;

        /// <summary>
        /// Returns the high threshold as an absolute value.
        /// </summary>
        public float HighValue => minValue + (maxValue - minValue) * highThreshold;


    }
}
