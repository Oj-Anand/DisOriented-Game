using UnityEngine;
using System;
using System.IO;

namespace DisOriented.Core
{
    ///<summary>
    ///Handles save file IO 
    ///</summary>
    public class SaveManager : Singleton<SaveManager>
    {
        private const string SAVE_FILENAME = "save_slot1.json";
        private const int CURRENT_SCHEMA = 1;

        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILENAME);

        //###################################
        //          PUBLIC API
        //###################################

        ///<summary>Save current game state to disk.</summary>
        public bool Save()
        {
            try
            {
                SaveData data = SaveData.CaptureCurrentState();
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"[  SAVEMANAGER] Saved to {SavePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SAVEMANAGER] Savef failed: {e.Message}");
                return false; 
            }
        }

        ///<summary>Load save file and apply to game state</summary>
        public bool Load()
        {
            if (!SaveExists())
            {
                Debug.LogWarning($"[SAVEMANAGER] No save file found.");
                return false;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                //Schema migration
                if (data.schemaVersion < CURRENT_SCHEMA)
                {
                    data = MigrateSave(data);
                }

                data.ApplyToGame();
                Debug.Log($"[SAVEMANAGER] Loaded save from {data.savedAt}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SAVEMANAGER] Load failed: {e.Message}");
                return false; 
            }
        }

        ///<summary>Checks if a save file exists on disk</summary>
        public bool SaveExists()
        {
            return File.Exists(SavePath);
        }

        ///<summary>Deelte save for new game</summary>
        public void DeleteSave()
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log($"[SAVEMANAGER] Save file deleted at {SavePath}"); 
            }
        }

        //###################################
        //          MIGRATION  
        //###################################

        private SaveData MigrateSave(SaveData oldData)
        {
            Debug.Log($"[SAVEMANAGER] Migrating save from v{oldData.schemaVersion} to v{CURRENT_SCHEMA}");

            //TODO: Add migration steps
            oldData.schemaVersion = CURRENT_SCHEMA;
            return oldData;
        }
    }
}
