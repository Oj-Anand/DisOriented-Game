using UnityEngine;
using DisOriented.Core;
using DisOriented.Core.Events;
using DisOriented.Data;

namespace DisOriented.Utilities
{
    /// <summary>
    /// Temporary validation script. Attach to any GO in Boot scene.
    /// Run with Play Mode. Check console for PASS/FAIL.
    /// Delete after Sprint 1 is validated (or keep for regression).
    /// </summary>
    public class Sprint1Tests : MonoBehaviour
    {
        private int _passCount = 0;
        private int _failCount = 0;

        private void Start()
        {
            Invoke(nameof(RunAllTests), 0.5f); // Wait for managers
        }

        private void RunAllTests()
        {
            Debug.Log("===== SPRINT 1 TESTS =====");

            TestResourceGetSet();
            TestResourceClamp();
            TestResourceEvents();
            TestCriticalThreshold();
            TestSaveLoadRoundTrip();
            TestLoadWithNoFile();
            TestResetAll();

            Debug.Log($"===== RESULTS: {_passCount} PASS, {_failCount} FAIL =====");
        }

        private void Assert(bool condition, string testName)
        {
            if (condition)
            {
                Debug.Log($"  PASS: {testName}");
                _passCount++;
            }
            else
            {
                Debug.LogError($"  FAIL: {testName}");
                _failCount++;
            }
        }

        private void TestResourceGetSet()
        {
            var rm = ResourceManager.Instance;
            rm.ResetAll();
            float startEnergy = rm.GetValue(ResourceType.Energy);
            rm.Modify(ResourceType.Energy, -10f);
            Assert(
                Mathf.Approximately(rm.GetValue(ResourceType.Energy), startEnergy - 10f),
                "Modify subtracts correctly"
            );
        }

        private void TestResourceClamp()
        {
            var rm = ResourceManager.Instance;
            rm.SetValue(ResourceType.Mood, 999f);
            Assert(rm.GetValue(ResourceType.Mood) <= 100f, "Clamp upper bound");
            rm.SetValue(ResourceType.Mood, -999f);
            Assert(rm.GetValue(ResourceType.Mood) >= 0f, "Clamp lower bound");
        }

        private void TestResourceEvents()
        {
            var rm = ResourceManager.Instance;
            rm.ResetAll();
            bool eventFired = false;
            void handler(ResourceChangeEvent e) { eventFired = true; }
            rm.OnResourceChanged += handler;
            rm.Modify(ResourceType.Swag, 5f);
            rm.OnResourceChanged -= handler;
            Assert(eventFired, "OnResourceChanged fires on Modify");
        }

        private void TestCriticalThreshold()
        {
            var rm = ResourceManager.Instance;
            rm.ResetAll();
            bool critFired = false;
            void handler(ResourceCriticalEvent e) { critFired = true; }
            rm.OnResourceCritical += handler;
            rm.SetValue(ResourceType.Energy, 5f); // Well below 20%
            rm.OnResourceCritical -= handler;
            Assert(critFired, "OnResourceCritical fires below threshold");
        }

        private void TestSaveLoadRoundTrip()
        {
            var rm = ResourceManager.Instance;
            rm.SetValue(ResourceType.Mood, 73f);
            rm.SetValue(ResourceType.Swag, 42f);
            rm.SetValue(ResourceType.Energy, 88f);
            rm.SetValue(ResourceType.Tummy, 16f);

            SaveManager.Instance.Save();
            rm.ResetAll(); // Wipe state
            SaveManager.Instance.Load(); // Restore

            Assert(Mathf.Approximately(rm.GetValue(ResourceType.Mood), 73f), "Save/Load Mood");
            Assert(Mathf.Approximately(rm.GetValue(ResourceType.Swag), 42f), "Save/Load Swag");
            Assert(Mathf.Approximately(rm.GetValue(ResourceType.Energy), 88f), "Save/Load Energy");
            Assert(Mathf.Approximately(rm.GetValue(ResourceType.Tummy), 16f), "Save/Load Tummy");
        }

        private void TestLoadWithNoFile()
        {
            SaveManager.Instance.DeleteSave();
            bool result = SaveManager.Instance.Load();
            Assert(!result, "Load returns false with no save file");
        }

        private void TestResetAll()
        {
            var rm = ResourceManager.Instance;
            rm.SetValue(ResourceType.Mood, 0f);
            rm.ResetAll();
            var def = rm.GetDefinition(ResourceType.Mood);
            Assert(
                Mathf.Approximately(rm.GetValue(ResourceType.Mood), def.startingValue),
                "ResetAll restores starting values"
            );
        }
    }
}
