using System;
using System.Collections.Generic;
using DisOriented.Data;

namespace DisOriented.Core
{
    /// <summary>
    /// Serializeable container for a single save file
    /// </summary>
    [Serializable]
    public class SaveData
    {
        // --- Schema --- 
        public int schemaVersion = 1;
        public string savedAt;

        // --- Resources ---
        public float mood;
        public float swag;
        public float energy;
        public float tummy;

        // ---- Time ----
        public int currentDay;       // 1-9
        public int currentPhase;     // 0=Morning, 1=Afternoon, 2=Evening, 3=Night

        /// <summary>
        /// Pack current game state into this SaveData instance.
        /// </summary>
        public static SaveData CaptureCurrentState()
        {
            var rm = ResourceManager.Instance;
            var tm = TimeManager.Instance;

            return new SaveData
            {
                schemaVersion = 1,
                savedAt = DateTime.UtcNow.ToString("o"),
                mood = rm.GetValue(ResourceType.Mood),
                swag = rm.GetValue(ResourceType.Swag),
                energy = rm.GetValue(ResourceType.Energy),
                tummy = rm.GetValue(ResourceType.Tummy),
                currentDay = tm.CurrentDay,
                currentPhase = (int)tm.CurrentPhase,
            };  
        }

        /// <summary>
        /// Apply this SaveData to all game managers.
        /// </summary>
        public void ApplyToGame()
        {
            var rm = ResourceManager.Instance;
            var state = new Dictionary<ResourceType, float>
            {
                { ResourceType.Mood,   mood },
                { ResourceType.Swag,   swag },
                { ResourceType.Energy, energy },
                { ResourceType.Tummy,  tummy },
            };
            rm.ImportState(state);

            var tm = TimeManager.Instance;
            tm.SetState(currentDay, (TimePhase)currentPhase); 
        }



    }
}


