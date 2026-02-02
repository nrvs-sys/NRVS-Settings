using NRVS.Store;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace NRVS.Settings
{
    /// <summary>
    /// A drop-in replacement for Unity's PlayerPrefs, using a DataCache to store key/value pairs of integers, floats, and strings.
    /// </summary>
    public class PlayerData : Singleton<PlayerData>
    {
        [Header("References")]

        [SerializeField]
        SettingsCacheStoreBehavior settingsCacheStoreBehavior;

        [SerializeField, Tooltip("How often (in seconds) to save if there are unsaved changes.")]
        float saveInterval = 10f;

        string platformDirectoryPath;
        SettingsCache playerData;

        float saveTimer = 0f;

        Task _pendingSave;
        readonly object _saveLock = new();

        protected override void OnSingletonInitialized()
        {
            platformDirectoryPath = StoreBehavior.GetPlatformDirectoryPath();
            playerData = settingsCacheStoreBehavior.Load(platformDirectoryPath);

            if (playerData == null)
            {
                playerData = new SettingsCache();
                settingsCacheStoreBehavior.Save(playerData, platformDirectoryPath);
            }

            // Enable fast lookups with Dictionary caches
            playerData.useLookupCache = true;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                Save();
        }

        private void Update()
        {
            // Periodically save if dirty
            if (saveInterval <= 0f)
                return;

            if (playerData == null || settingsCacheStoreBehavior == null)
                return;

            // Don't save during timescale pauses
            if (Time.timeScale == 0f)
                return;

            if (!playerData.isDirty)
                return;

            saveTimer += Time.unscaledDeltaTime;

            if (saveTimer >= saveInterval)
            {
                Save();
                saveTimer = 0f;
            }
        }

        public static int GetInt(string key, int defaultValue = 0) => Instance.playerData.GetInt(key, defaultValue);
        public static void SetInt(string key, int value) => Instance.playerData.SetInt(key, value);
        public static float GetFloat(string key, float defaultValue = 0f) => Instance.playerData.GetFloat(key, defaultValue);
        public static void SetFloat(string key, float value) => Instance.playerData.SetFloat(key, value);
        public static string GetString(string key, string defaultValue = "") => Instance.playerData.GetString(key, defaultValue);
        public static void SetString(string key, string value) => Instance.playerData.SetString(key, value);
        public static bool HasKey(string key) => Instance.playerData.HasKey(key);
        public static void DeleteKey(string key) => Instance.playerData.DeleteKey(key);
        public static void DeleteAll() => Instance.playerData.DeleteAll();

        public static void Save() => _ = SaveAsync();

        public static Task SaveAsync()
        {
            var i = Instance;

            if (i == null || i.playerData == null)
                return Task.CompletedTask;

            if (!i.playerData.isDirty)
                return Task.CompletedTask;

            Task doSave()
            {
                // coalesce: if a save is already running, return it
                lock (i._saveLock)
                {
                    if (i._pendingSave != null && !i._pendingSave.IsCompleted)
                        return i._pendingSave;

                    i._pendingSave = i.settingsCacheStoreBehavior.SaveAsync(i.playerData, i.platformDirectoryPath);
                    return i._pendingSave.ContinueWith(t =>
                    {
                        i.playerData.Flush();
                        return t; // propagate exceptions
                    }).Unwrap();
                }
            }

            return doSave();
        }
    }
}
