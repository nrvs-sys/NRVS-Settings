using GameKit.Dependencies.Utilities;
using NRVS.Store;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NRVS.Settings
{
    /// <summary>
    /// A simple data cache for storing key/value pairs of integers, floats, and strings, with an API to match Unity's PlayerPrefs.
    /// 
    /// A Dictionary cache can be used for faster lookups. Toggle `useLookupCache` to enable this feature.
    /// </summary>
    [Serializable]
    public class SettingsCache : IStorable
    {
        #region Entry Structs

        [Serializable]
        public struct IntEntry { public string key; public int value; }
        [Serializable]
        public struct FloatEntry { public string key; public float value; }
        [Serializable]
        public struct StringEntry { public string key; public string value; }

        #endregion

        #region Serialized Storage Lists

        [Tooltip("Int key/value pairs")]
        public List<IntEntry> intEntries = new List<IntEntry>();
        [Tooltip("Float key/value pairs")]
        public List<FloatEntry> floatEntries = new List<FloatEntry>();
        [Tooltip("String key/value pairs")]
        public List<StringEntry> stringEntries = new List<StringEntry>();

        #endregion

        #region Runtime Caches

        /// <summary>
        /// If true, Dictionary caches are used for fast lookups of key/value pairs.
        /// </summary>
        [NonSerialized]
        public bool useLookupCache = false;

        // Lazy-initialized for fast lookup
        // (not serialized)
        Dictionary<string, int> intCache;
        Dictionary<string, float> floatCache;
        Dictionary<string, string> stringCache;

        void LazyInitializeLookupCache()
        {
            if (!useLookupCache || intCache != null) return;

            intCache = CollectionCaches<string, int>.RetrieveDictionary();
            floatCache = CollectionCaches<string, float>.RetrieveDictionary();
            stringCache = CollectionCaches<string, string>.RetrieveDictionary();

            foreach (var e in intEntries) intCache[e.key] = e.value;
            foreach (var e in floatEntries) floatCache[e.key] = e.value;
            foreach (var e in stringEntries) stringCache[e.key] = e.value;
        }

        public void DisposeLookupCache()
        {
            CollectionCaches<string, int>.Store(intCache);
            CollectionCaches<string, float>.Store(floatCache);
            CollectionCaches<string, string>.Store(stringCache);

            intCache = null;
            floatCache = null;
            stringCache = null;
        }

        #endregion

        #region PlayerPrefs API

        public bool isDirty { get; private set; } = false;

        public int GetInt(string key, int defaultValue = 0)
        {
            LazyInitializeLookupCache();

            if (intCache != null)
                return intCache.TryGetValue(key, out var v) ? v : defaultValue;
            else
            {
                for (var i = 0; i < intEntries.Count; i++)
                {
                    if (intEntries[i].key == key)
                        return intEntries[i].value;
                }
                return defaultValue;
            }
        }

        public void SetInt(string key, int value)
        {
            LazyInitializeLookupCache();

            if (intCache != null)
            {
                if (intCache.TryGetValue(key, out var existing) && existing == value)
                    return;

                intCache[key] = value;
            }

            floatCache?.Remove(key);
            stringCache?.Remove(key);

            intEntries.RemoveAll(e => e.key == key);
            floatEntries.RemoveAll(e => e.key == key);
            stringEntries.RemoveAll(e => e.key == key);

            intEntries.Add(new IntEntry { key = key, value = value });

            isDirty = true;
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            LazyInitializeLookupCache();

            if (floatCache != null)
                return floatCache[key];
            else
            {
                for (var i = 0; i < floatEntries.Count; i++)
                {
                    if (floatEntries[i].key == key)
                        return floatEntries[i].value;
                }
                return defaultValue;
            }
        }

        public void SetFloat(string key, float value)
        {
            LazyInitializeLookupCache();

            if (floatCache != null)
            {
                if (floatCache.TryGetValue(key, out var existing) && Math.Abs(existing - value) < Mathf.Epsilon)
                    return;

                floatCache[key] = value;
            }

            intCache?.Remove(key);
            stringCache?.Remove(key);

            floatEntries.RemoveAll(e => e.key == key);
            intEntries.RemoveAll(e => e.key == key);
            stringEntries.RemoveAll(e => e.key == key);

            floatEntries.Add(new FloatEntry { key = key, value = value });

            isDirty = true;
        }

        public string GetString(string key, string defaultValue = "")
        {
            LazyInitializeLookupCache();

            if (stringCache != null)
                return stringCache.TryGetValue(key, out var v) ? v : defaultValue;
            else
            {
                for (var i = 0; i < stringEntries.Count; i++)
                {
                    if (stringEntries[i].key == key)
                        return stringEntries[i].value;
                }
                return defaultValue;
            }
        }

        public void SetString(string key, string value)
        {
            LazyInitializeLookupCache();

            if (stringCache != null)
            {
                if (stringCache.TryGetValue(key, out var existing) && existing == value)
                    return;

                stringCache[key] = value;
            }

            intCache?.Remove(key);
            floatCache?.Remove(key);

            stringEntries.RemoveAll(e => e.key == key);
            intEntries.RemoveAll(e => e.key == key);
            floatEntries.RemoveAll(e => e.key == key);

            stringEntries.Add(new StringEntry { key = key, value = value });

            isDirty = true;
        }

        public bool HasKey(string key)
        {
            LazyInitializeLookupCache();

            if (intCache != null)
            {
                return intCache.ContainsKey(key)
                    || floatCache.ContainsKey(key)
                    || stringCache.ContainsKey(key);
            }
            else
            {
                return intEntries.Exists(e => e.key == key)
                    || floatEntries.Exists(e => e.key == key)
                    || stringEntries.Exists(e => e.key == key);
            }
        }

        public void DeleteKey(string key)
        {
            LazyInitializeLookupCache();

            intCache?.Remove(key);
            floatCache?.Remove(key);
            stringCache?.Remove(key);

            intEntries.RemoveAll(e => e.key == key);
            floatEntries.RemoveAll(e => e.key == key);
            stringEntries.RemoveAll(e => e.key == key);

            isDirty = true;
        }

        public void DeleteAll()
        {
            LazyInitializeLookupCache();

            intCache?.Clear(); floatCache?.Clear(); stringCache?.Clear();
            intEntries.Clear(); floatEntries.Clear(); stringEntries.Clear();

            isDirty = true;
        }

        public void Flush()
        {
            isDirty = false;
        }

        #endregion

        #region IStorable Methods

        public uint GetStoreVersion() => 0;

        public uint GetLatestStoreVersion() => 0;

        public void UpdateStoreVersion() { }

        #endregion
    }
}
