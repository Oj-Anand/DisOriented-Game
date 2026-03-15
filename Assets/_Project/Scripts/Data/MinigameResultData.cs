using System.Collections.Generic;
using DisOriented.Core;

namespace DisOriented.Data
{
    /// <summary>Results from a completed minigame. Produced by the minigame,</summary>
    public class MinigameResultData
    {
        public string MinigameName;
        public bool Succeeded;
        public float ScorePercentage;  // 0-1
        public int StarsEarned;        // 0-3
        public int CashCollected;      //

        /// <summary>Resource changes to apply (positive = reward).</summary>
        public Dictionary<ResourceType, float> ResourceDeltas;

        public MinigameResultData()
        {
            ResourceDeltas = new Dictionary<ResourceType, float>();
        }

        /// <summary>Apply all resource changes to the ResourceManager.</summary>
        public void ApplyRewards()
        {
            var rm = ResourceManager.Instance;
            if (rm == null) return;

            foreach (var kvp in ResourceDeltas)
            {
                rm.Modify(kvp.Key, kvp.Value);
            }
        }
    }
}
