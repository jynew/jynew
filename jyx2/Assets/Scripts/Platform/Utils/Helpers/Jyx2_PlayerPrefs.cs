using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;


namespace Jyx2
{
    public class Jyx2_PlayerPrefsData
    {
        public Dictionary<string, int> m_IntDic { get; } = new Dictionary<string, int>();

        public Dictionary<string, bool> m_BoolDic { get; } = new Dictionary<string, bool>();

        public Dictionary<string, float> m_FloatDic { get; } = new Dictionary<string, float>();

        public Dictionary<string, string> m_StringDic { get; } = new Dictionary<string, string>();

        public void Clear()
        {
            m_IntDic.Clear();
            m_BoolDic.Clear();
            m_FloatDic.Clear();
            m_StringDic.Clear();
        }

        public bool HasKey(string key)
        {
            return m_StringDic.ContainsKey(key) || m_BoolDic.ContainsKey(key) || m_IntDic.ContainsKey(key) || m_FloatDic.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            if (m_IntDic.ContainsKey(key))
            {
                m_IntDic.Remove(key);
            }
            if (m_StringDic.ContainsKey(key))
            {
                m_StringDic.Remove(key);
            }
            if (m_BoolDic.ContainsKey(key))
            {
                m_BoolDic.Remove(key);
            }
            if (m_FloatDic.ContainsKey(key))
            {
                m_FloatDic.Remove(key);
            }
        }
    }


    public static class Jyx2_PlayerPrefs
    {
        private static readonly string SavePath = Path.Combine(Application.persistentDataPath,
#if UNITY_EDITOR
            "PlayerPrefs/Jyx2_PlayerPrefs_Editor.json");
#else
            "PlayerPrefs/Jyx2_PlayerPrefs.json");
#endif
        private static Jyx2_PlayerPrefsData m_PrefsData = new Jyx2_PlayerPrefsData();

        private static bool m_Init = false;

        public static void CheckInit()
        {
            if (m_Init)
                return;
            m_Init = true;
            Debug.Log("Initialize Jyx2_PlayerPrefs...");
            EnsureFileExists();
            ReLoad();
        }

        private static void EnsureFileExists()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    var dirName = Path.GetDirectoryName(SavePath);
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                    File.WriteAllText(SavePath, "");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("初始化PlayerPrefs文件失败");
                Debug.LogError(ex);
            }
        }

        public static void ReLoad()
        {
            m_PrefsData.Clear();
            try
            {
                string jsonText;
                if (Application.isMobilePlatform)
                {
                    jsonText = PlayerPrefs.GetString(SavePath);    
                }
                else
                {
                    jsonText = File.ReadAllText(SavePath);    
                }

                var newPrefs = JsonConvert.DeserializeObject<Jyx2_PlayerPrefsData>(jsonText);
                if(newPrefs != null)
                {
                    m_PrefsData = newPrefs;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError("Load PlayerPrefs from json failed");
            }
        }

        public static void DeleteAll()
        {
            m_PrefsData.Clear();
#if UNITY_EDITOR
            Save();
#endif
        }

        public static void DeleteKey(string key)
        {
            CheckInit();
            m_PrefsData.DeleteKey(key);
#if UNITY_EDITOR
            Save();
#endif
        }


        public static bool HasKey(string key)
        {
            CheckInit();
            return m_PrefsData.HasKey(key);
        }

        public static void Save()
        {
            CheckInit();
            try
            {
                var jsonText = JsonConvert.SerializeObject(m_PrefsData, Formatting.Indented);
                if (Application.isMobilePlatform)
                {
                    PlayerPrefs.SetString(SavePath, jsonText);
                    PlayerPrefs.Save();    
                }
                else
                {
                    File.WriteAllText(SavePath, jsonText);    
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError("Save PlayerPrefs to json failed");
            }
        }

#region Get & Set API
        public static float GetFloat(string key, float defaultValue = 0)
        {
            CheckInit();
            return GetValue(m_PrefsData.m_FloatDic, key, defaultValue);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            CheckInit();
            return GetValue(m_PrefsData.m_IntDic, key, defaultValue);
        }


        public static string GetString(string key, string defaultValue = "")
        {
            CheckInit();
            return GetValue(m_PrefsData.m_StringDic, key, defaultValue);
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            CheckInit();
            return GetValue(m_PrefsData.m_BoolDic, key, defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            CheckInit();
            SetValue(m_PrefsData.m_FloatDic, key, value);
        }
        public static void SetInt(string key, int value)
        {
            CheckInit();
            SetValue(m_PrefsData.m_IntDic, key, value);
        }
        public static void SetString(string key, string value)
        {
            CheckInit();
            SetValue(m_PrefsData.m_StringDic, key, value);
        }

        public static void SetBool(string key, bool value)
        {
            CheckInit();
            SetValue(m_PrefsData.m_BoolDic, key, value);
        }

        private static void SetValue<T>(Dictionary<string, T> dic, string key, T val)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = val;
            }
            else
            {
                dic.Add(key, val);
            }

            Save();
        }

        private static T GetValue<T>(Dictionary<string, T> dic, string key, T defaultValue)
        {
            if (dic.ContainsKey(key))
                return dic[key];
            return defaultValue;
        }

#endregion
    }

}