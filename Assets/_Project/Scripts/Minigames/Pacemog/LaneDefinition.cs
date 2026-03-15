using UnityEngine;

namespace DisOriented.Minigames.Pacemog
{
    /// <summary>Defines a single lane's properties.</summary>
    [System.Serializable]
    public class LaneDefinition
    {
        [Tooltip("World-space X position of this lane center")]
        public float xPosition;

        [Tooltip("True = oncoming traffic (left 2 lanes)")]
        public bool isOncoming;

        [Tooltip("Score multiplier for this lane (oncoming = higher)")]
        public float scoreMultiplier = 1f;

        [Tooltip("Display name for debug/UI")]
        public string laneName;
    }
}

