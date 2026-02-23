using UnityEngine;
using DisOriented.Data;

namespace DisOriented.Core.Events
{
    ///<summary>
    ///Payload for resource change events. 
    ///Contains all neccessary context for listeners which then react as needed
    ///</summary>
    public readonly struct ResourceChangeEvent
    {
        public readonly ResourceType Type;
        public readonly float OldValue;
        public readonly float NewValue;
        public readonly float Delta;        // NewValue - OldValue
        public readonly float NormalizedValue; // 0-1 range
        public readonly bool IsCritical;    // Below critical threshold
        public readonly bool IsHigh;        // Above high threshold

        public ResourceChangeEvent(
            ResourceType type,
            float oldValue,
            float newValue,
            float normalizedValue,
            bool isCritical,
            bool isHigh)
        {
            Type = type;
            OldValue = oldValue;
            NewValue = newValue;
            Delta = newValue - oldValue;
            NormalizedValue = normalizedValue;
            IsCritical = isCritical;
            IsHigh = isHigh;
        }

    }

    ///<summary>
    ///Payload for criitical resource threshold events 
    ///Fires once when a resource crosses a critical threshold downwards
    ///</summary>
    public readonly struct ResourceCriticalEvent
    {
        public readonly ResourceType Type;
        public readonly float CurrentValue;
        public readonly float ThresholdValue;

        public ResourceCriticalEvent(
            ResourceType type,
            float currentValue,
            float thresholdValue)
        {
            Type = type;
            CurrentValue = currentValue;
            ThresholdValue = thresholdValue;
        }

    }

}


