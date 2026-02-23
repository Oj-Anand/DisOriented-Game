using UnityEngine;
using System;
using System.Collections.Generic;
using DisOriented.Data;
using DisOriented.Core.Events; 

namespace DisOriented.Core
{
    ///<summary>
    ///Singleton resource manager for all player resources 
    ///Fires events on change and owns all resources 
    ///</summary> 
    public class ResourceManager: Singleton<ResourceManager>
    {
        // --- INSPECTOR FIELDS --- 
        [Header("Resource Definitions")]
        [SerializeField] private ResourceDefinition[] resourceDefinitions;

        //--- EVENTS ---

        /// <summary>
        /// Fires on every resource value change
        /// </summary>
        public event Action<ResourceChangeEvent> OnResourceChanged; 

        ///<summary>
        ///Fires once when a resource crosses below its critical threshold
        ///Doesnt refire if resource stays below critcal threshold
        ///</summary>
        public event Action<ResourceCriticalEvent> OnResourceCritical;

        /// <summary>
        /// Fires ONCE when a resource recovers above its critical threshold.
        /// </summary>
        public event Action<ResourceType> OnResourceRecovered;

        // --- INTERNAL STATE ---
        private Dictionary<ResourceType, float> _values;
        private Dictionary<ResourceType, ResourceDefinition> _definitions;
        private HashSet<ResourceType> _criticalResources; //hashset cause it only stores unique values

        // ######################################
        //            INITIALIZATION 
        // ######################################

        protected override void OnInitialize()
        {
            _values = new Dictionary<ResourceType, float>();
            _definitions = new Dictionary<ResourceType, ResourceDefinition>();
            _criticalResources = new HashSet<ResourceType>();

            foreach (var def in resourceDefinitions)
            {
                if (def == null)
                {
                    Debug.LogError("[RESOURCEMANAGER] Null definition in array!");
                    continue; 
                }
                _definitions[def.resourceType] = def;
                _values[def.resourceType] = def.startingValue;

            }

            ValidateSetup();
        }

        private void ValidateSetup()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                if (!_definitions.ContainsKey(type))
                {
                    Debug.LogError($"[ResourceManager] Missing definition for {type}!");
                }
                    
            }
        }


        // ############################################
        //              PUBLIC API - READ
        // ############################################

        /// <summary>Get the current absolute value of a resource.</summary>
        public float GetValue(ResourceType type)
        {
            return _values.TryGetValue(type, out float val) ? val : 0f;
        }

        /// <summary>Get normalized value (0-1) of a resource.</summary>
        public float GetNormalized(ResourceType type)
        {
            if (!_definitions.TryGetValue(type, out var def)) return 0f;
            float range = def.maxValue - def.minValue;
            if (range <= 0f) return 0f;
            return Mathf.Clamp01((_values[type] - def.minValue) / range);
        }

        /// <summary>Check if a resource is currently below critical.</summary>
        public bool IsCritical(ResourceType type)
        {
            return _criticalResources.Contains(type);
        }


        /// <summary>Get the definition SO for a resource type.</summary>
        public ResourceDefinition GetDefinition(ResourceType type)
        {
            return _definitions.TryGetValue(type, out var def) ? def : null;
        }


        // ###########################################
        //              PUBLIC API - WRITE  
        // ###########################################

        /// <summary>
        /// Add or subtract from a resource. Positive = gain, negative = cost.
        /// This is the PRIMARY method for changing resources.
        /// </summary>
        public void Modify(ResourceType type, float delta)
        {
            if (!_values.ContainsKey(type)) return;
            float oldValue = _values[type];
            SetInternal(type, oldValue + delta);
        }

        /// <summary>
        /// Set a resource to an exact value. Use sparingly;
        /// prefer Modify() for gameplay changes.
        /// </summary>
        public void SetValue(ResourceType type, float value)
        {
            if (!_values.ContainsKey(type)) return;
            SetInternal(type, value);
        }

        /// <summary>Reset all resources to their starting values.</summary>
        public void ResetAll()
        {
            foreach (var kvp in _definitions)
            {
                SetInternal(kvp.Key, kvp.Value.startingValue);
            }
            _criticalResources.Clear();
        }


        // #############################################
        //                    INTERNAL
        // #############################################

        private void SetInternal(ResourceType type, float newValue)
        {
            var def = _definitions[type];
            float oldValue = _values[type];

            // Clamp to bounds
            newValue = Mathf.Clamp(newValue, def.minValue, def.maxValue);

            // Skip if no actual change 
            if (Mathf.Approximately(oldValue, newValue)) return;

            _values[type] = newValue;

            float normalized = GetNormalized(type);
            bool isCritical = newValue <= def.CriticalValue;
            bool isHigh = newValue >= def.HighValue;

            // Fire general change event
            OnResourceChanged?.Invoke(new ResourceChangeEvent(
                type, oldValue, newValue, normalized, isCritical, isHigh
            ));

            // Handle critical threshold crossing
            if (isCritical && !_criticalResources.Contains(type))
            {
                _criticalResources.Add(type);
                OnResourceCritical?.Invoke(new ResourceCriticalEvent(
                    type, newValue, def.CriticalValue
                ));
            }
            else if (!isCritical && _criticalResources.Contains(type))
            {
                _criticalResources.Remove(type);
                OnResourceRecovered?.Invoke(type);
            }
        }

        // ##################################################
        //                  SAVE / LOAD  
        // ##################################################

        /// <summary>Export current resource values for saving.</summary>
        public Dictionary<ResourceType, float> ExportState()
        {
            return new Dictionary<ResourceType, float>(_values);
        }

        /// <summary>Import saved resource values.</summary>
        public void ImportState(Dictionary<ResourceType, float> savedValues)
        {
            foreach (var kvp in savedValues)
            {
                if (_values.ContainsKey(kvp.Key))
                    SetInternal(kvp.Key, kvp.Value);
            }
        }


    }


}
