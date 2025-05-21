//Requires com.unity.nuget.newtonsoft-json package

#if ARMONY_SERIALIZATION
using System;
using System.Collections.Generic;
using System.IO;
using Armony.Utilities;
using Newtonsoft.Json;
using UnityEngine;

namespace ArmanDoesStuff.Core
{
    public static class DataOperations
    {
        private static string BaseDirectory => Path.Combine(Application.persistentDataPath, "GameData");
        private static string GetFilename(string _fileName) => Path.Combine(BaseDirectory, $"{_fileName}.gData");
        private static string GetPrefsFilename(string _fileName) => Path.Combine(BaseDirectory, $"{_fileName}.pData");

        public static void SaveData<T>(T _saveData, string _fileName = "Main")
        {
            _fileName = GetFilename(_fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(_fileName)!);
            string jsonData = JsonConvert.SerializeObject(_saveData);
            byte[] encrypted = EncrypterAes.EncryptStringToBytesAes(jsonData);
            File.WriteAllBytes(_fileName, encrypted);
        }


        public static bool LoadData<T>(out T _data, string _fileName = "Main")
        {
            _fileName = GetFilename(_fileName);
            if (File.Exists(_fileName))
            {
                byte[] encrypted = File.ReadAllBytes(_fileName);
                string jsonData = EncrypterAes.DecryptStringFromBytesAes(encrypted);
                _data = JsonConvert.DeserializeObject<T>(jsonData);
                return true;
            }

            if (typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                _data = default;
            }
            else
            {
                _data = Activator.CreateInstance<T>();
            }

            return false;
        }

        //My own version of player prefs - Includes things like default values - Requires .Net 4
        //Uses a Dictionary of objects to store most kinds of data
        //Some things may nor work on Android - More testing needed (Prefs does not seems to work)
        public static T GetPref<T>(string _key, T _defaultVal, string _fileName)
        {
            if (!GetPrefDict(_fileName).TryGetValue(_key, out object value)) return _defaultVal;
            string temp = value.ToString();
            return JsonConvert.DeserializeObject<T>(temp);
        }

        public static void SetPref<T>(string _key, T _value, string _fileName)
        {
            Dictionary<string, object> dict = GetPrefDict(_fileName);
            dict[_key] = JsonConvert.SerializeObject(_value);
            SaveData(dict, GetPrefsFilename(_fileName));
        }

        private static Dictionary<string, object> GetPrefDict(string _fileName)
        {
            LoadData(out Dictionary<string, object> dict, GetPrefsFilename(_fileName));
            return dict ?? new Dictionary<string, object>();
        }

        public static void ClearData()
        {
            if (Directory.Exists(BaseDirectory))
            {
                try
                {
                    Directory.Delete(BaseDirectory, true);
                    Debug.Log("GameData deleted.");
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to delete GameData folder:\n{ex.Message}");
                }
            }
            else
            {
                Debug.Log("GameData not found.");
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void DeleteFile(string _fileName, bool _prefs = false)
        {
            if (_prefs)
                _fileName = GetPrefsFilename(_fileName);
            File.Delete(Application.persistentDataPath + _fileName);
        }
    }
}
#endif