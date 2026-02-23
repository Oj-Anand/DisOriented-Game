using UnityEngine;
using System;
using DisOriented.Data;
using DisOriented.Core.Events;

namespace DisOriented.Core
{
    ///<summary>
    ///Singleton manager tracking the 9 day , 4 phase per day game clock 
    ///Fires events and enforcesd passiove resource drain on each advance
    ///</summary>


    public class TimeManager : Singleton<TimeManager>
    {
        // ---- Inspector ----

        [Header("Configuration")]
        [SerializeField] private TimeDefinition timeDefinition;

        // ---- Events ----

        ///<summary>Fires on every phase advance</summary>
        public event Action<TimeAdvancedEvent> OnTimeAdvanced;

        ///<summary>Fires once when a new day begins</summary>
        public event Action<DayChangedEvent> OnDayChanged;

        ///<summary>Fires once when the entire week is complete</summary>
        public event Action<WeekCompleteEvent> OnWeekComplete;

        //---- State ----
        private int _currentDay; //1 - total days
        private TimePhase _currentPhase;
        private bool _weekComplete;
        private int _timeSlotIndex; //0 - 35

        //---- Public read properties ----
        public int CurrentDay => _currentDay;
        public TimePhase CurrentPhase => _currentPhase;
        public bool WeekComplete => _weekComplete;
        public int TimeSlotIndex => _timeSlotIndex;
        public TimeDefinition Definition => timeDefinition;

        ///<summary>Formated display string: "Day 1 - Monday"</summary>
        public string DayDisplayString => $"Day {_currentDay} - {timeDefinition.GetDayName(_currentDay)}";

        ///<summary>Formatted phase display: "Morning"</summary>
        public string PhaseDisplayString => timeDefinition.GetPhaseConfig(_currentPhase).displayName;

        //####################################
        //          INITIALIZATION
        //####################################
        protected override void OnInitialize()
        {
            ResetToStart(); 
        }

        ///<summary>Reset time to D1, Morning</summary>
        public void ResetToStart()
        {
            _currentDay = 1;
            _currentPhase = TimePhase.Morning; 
            _weekComplete = false;
            _timeSlotIndex = 0;
        }


        //#####################################
        //            PUBLIC API    
        //#####################################
        ///<summary>
        ///Advance time by one phase
        ///Morn -> Noon -> Eve -> Night -> Morn
        ///Fires events, applies passive drain 
        ///returns flase if week is already complete
        ///</summary>
        public bool AdvanceTime()
        {
            if (_weekComplete)
            {
                Debug.LogWarning("[TIMEMANAGER] Week already complete");
                return false; 
            }

            int oldDay = _currentDay;
            TimePhase oldPhase = _currentPhase;

            //Calculate next phase
            if (_currentPhase == TimePhase.Night)
            {
                //EOD - advance to next morning
                _currentDay++;
                _currentPhase = TimePhase.Morning;
            }
            else 
            {
                _currentPhase = (TimePhase)((int)_currentPhase + 1); //normal phase advance
            }

            _timeSlotIndex++;

            //Apply passive drain to resources
            ApplyPassiveDrain();

            if (_currentDay > timeDefinition.totalDays)
            {
                _weekComplete = true;
                _currentDay = timeDefinition.totalDays;
                _currentPhase = TimePhase.Night; //stay at final 

                OnWeekComplete?.Invoke(new WeekCompleteEvent(_timeSlotIndex));
                Debug.Log("[TIMEMANAGER] Week Complete ! ");
                return true;
            }

            //Fire time advanced event 
            OnTimeAdvanced?.Invoke(new TimeAdvancedEvent(oldDay, oldPhase, _currentDay, _currentPhase, _timeSlotIndex));

            //Fire Day changed event if applicable 
            if (oldDay != _currentDay)
            {
                string dayName = timeDefinition.GetDayName(_currentDay);
                OnDayChanged?.Invoke(new DayChangedEvent(oldDay, _currentDay, dayName));
                Debug.Log($"[TIMEMANAGER] Now: Day {_currentDay}, {_currentPhase}");
                return true;
            }

            Debug.Log($"[TIMEMANAGER] Now: Day {_currentDay}, {_currentPhase}");
            return true; 

        }

        ///<summary>
        ///Directly set time state
        ///used by save system
        ///</summary>
        public void SetState(int day, TimePhase phase)
        {
            _currentDay = Mathf.Clamp(day, 1, timeDefinition.totalDays);
            _currentPhase = phase;  
            _timeSlotIndex = ((_currentDay - 1)*4) + (int)_currentPhase;
            _weekComplete = (_currentDay >= timeDefinition.totalDays && _currentPhase == TimePhase.Night); 
        }

        ///<summary>
        ///Get linear progress through the week as a float bw 0-1
        ///used by UI
        ///</summary>
        public float GetWeekProgress()
        {
            return (float)_timeSlotIndex / (timeDefinition.TotalTimeSlots - 1); 
        }

        //###############################
        //        PASSIVE DRAIN 
        //###############################

        ///<summary>
        ///Apply each resources passive drain 
        ///On every time advance, gets drain values from resource SOs
        ///</summary>
        private void ApplyPassiveDrain()
        {
            var rm = ResourceManager.Instance;
            if (rm == null) return;

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                var def = rm.GetDefinition(type);
                if (def != null && def.passiveDrainPerPhase > 0f)
                {
                    rm.Modify(type, -def.passiveDrainPerPhase); 
                }
            }
        }

        //###############################
        //        SAVE/LOAD  
        //###############################

        ///<summary>
        ///Export time state for saving
        ///</summary>
        public (int day, int phase) ExportState()
        {
            return (_currentDay, (int)_currentPhase); 
        }
    }
}


