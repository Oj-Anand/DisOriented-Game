using System;
using UnityEngine;

namespace DisOriented.Data
{
    [CreateAssetMenu(fileName = "TimeDefinition", menuName = "DisOriented/TimeDefinition")]
    public class TimeDefinition : ScriptableObject
    {
        [Header("Week Structure")]
        [Tooltip("Total number of days in the game")]
        public int totalDays = 9;

        [Tooltip("Display names for each day, index 0 = day 1")]
        public string[] dayNames = new string[]
        {
            "Monday", "Tuesday", "Wednesday", "Thursday",
            "Friday", "Saturday", "Sunday", "Monday", "Tuesday"

        };

        [Header("Phase Config")]
        public PhaseConfig[] phases = new PhaseConfig[]
        {
            new PhaseConfig { phase = TimePhase.Morning,   displayName = "Morning",   colorHex = "FFD54F" },
            new PhaseConfig { phase = TimePhase.Afternoon, displayName = "Afternoon", colorHex = "FF8A65" },
            new PhaseConfig { phase = TimePhase.Evening,   displayName = "Evening",   colorHex = "7986CB" },
            new PhaseConfig { phase = TimePhase.Night,     displayName = "Night",     colorHex = "5C6BC0" },

        };

        ///<summary>
        ///Get the display name for a given day number
        ///Return Day X as fallback
        ///</summary>
        public string GetDayName(int dayNumber)
        {
            int index = dayNumber - 1; 
            if(index >= 0 && index < dayNames.Length) return dayNames[index];
            return $"Day {dayNumber}";
        }

        ///<summary>
        ///Get the PhaseConfig for a given TimePhase
        ///</summary>

        public PhaseConfig GetPhaseConfig(TimePhase phase)
        {
            int index = (int)phase;
            if (index >= 0 && index < phases.Length) return phases[index];
            return new PhaseConfig { phase = phase, displayName = phase.ToString(), colorHex = "FFFFFF" };
        }

        ///<summary>
        ///Total number of time slots in the entire game
        ///</summary>

        public int TotalTimeSlots => totalDays * 4;

        [Serializable]
        public class PhaseConfig
        {
            public TimePhase phase;
            public string displayName;

            [Tooltip("Hex color for the HUD indicator tint")]
            public string colorHex;

            [Tooltip("Optional icon sprite fo rthis time of day")]
            public Sprite icon;

            ///<summary>Parse colorHex into a Unity Color.</summary>
            public Color GetColor()
            {
                if(ColorUtility.TryParseHtmlString($"#{colorHex}", out Color c))
                    return c;
                return Color.white; 
            }
        }

    }
}

