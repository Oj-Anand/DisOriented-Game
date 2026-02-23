using DisOriented.Data;

namespace DisOriented.Core.Events
{
    ///<summary>
    ///Fired every time the time phase advances
    ///</summary>

    public readonly struct TimeAdvancedEvent
    {
        public readonly int OldDay;
        public readonly TimePhase OldPhase;
        public readonly int NewDay;
        public readonly TimePhase NewPhase;
        public readonly bool DayChanged;     // True if we moved to a new day
        public readonly int TimeSlotIndex;   // 0-35, linear index

        public TimeAdvancedEvent(
            int oldDay, TimePhase oldPhase,
            int newDay, TimePhase newPhase,
            int timeSlotIndex)
        {
            OldDay = oldDay;
            OldPhase = oldPhase;
            NewDay = newDay;
            NewPhase = newPhase;
            DayChanged = (oldDay != newDay);
            TimeSlotIndex = timeSlotIndex;
        }

    }

    ///<summary>
    ///Fired once when a new day begins
    ///</summary>

    public readonly struct DayChangedEvent
    {
        public readonly int PreviousDay;
        public readonly int NewDay;
        public readonly string DayName;

        public DayChangedEvent(int previousDay, int newDay, string dayName)
        {
            PreviousDay = previousDay;
            NewDay = newDay;
            DayName = dayName;
        }
    }

    ///<summary>
    ///Fired once when the 9 day cycle finishes
    ///</summary>
    public readonly struct WeekCompleteEvent
    {
        public readonly int TotalTimeSlotsUsed;

        public WeekCompleteEvent(int totalTimeSlotsUsed)
        {
            TotalTimeSlotsUsed = totalTimeSlotsUsed;
        }

    }

}
