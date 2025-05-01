#if ARMONY_SERIALIZATION
using System;
using System.Collections.Generic;
using System.IO;
using Armony.Utilities;
using UnityEngine;

namespace ArmanDoesStuff.Core
{
    public static class DataOperations
    {
        private static string GetFilename(string fileName) => $"{Application.persistentDataPath}/{fileName}";
        private static string GetPrefsFilename(string fileName) => $"{GetFilename(fileName)}Prefs";

        public static bool LoadActual<T>(string fileName, out T data)
        {
            fileName = GetFilename(fileName);
            if (File.Exists(fileName))
            {
                byte[] encrypted = File.ReadAllBytes(fileName);
                string jsonData = EncrypterAes.DecryptStringFromBytesAes(encrypted);
                data = JsonConvert.DeserializeObject<T>(jsonData);
                return true;
            }
            if (typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null)
            {
                data = default;
            }
            else
            {
                data = Activator.CreateInstance<T>();
            }
            return false;
        }

        public static void SaveActual<T>(T saveData, string fileName)
        {
            fileName = GetFilename(fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            string jsonData = JsonConvert.SerializeObject(saveData);
            byte[] encrypted = EncrypterAes.EncryptStringToBytesAes(jsonData);
            File.WriteAllBytes(fileName, encrypted);
        }

        //My own version of player prefs - Includes things like default values - Requires .Net 4
        //Uses a Dictionary of objects to store most kinds of data
        //Some things may nor work on Android - More testing needed (Prefs does not seems to work)
        public static T GetPref<T>(string key, T defaultVal, string fileName)
        {
            Dictionary<string, object> dict = GetPrefDict(fileName);

            if (dict.ContainsKey(key))
            {
                string temp = dict[key].ToString();
                return JsonConvert.DeserializeObject<T>(temp);
            }
            return defaultVal;
        }

        public static void SetPref<T>(string key, T value, string fileName)
        {
            Dictionary<string, object> dict = GetPrefDict(fileName);
            dict[key] = JsonConvert.SerializeObject(value);
            SaveActual(dict, GetPrefsFilename(fileName));
        }

        //May now be able to use arrays in the regular Get/Set pref method - haven't tested since changing it
        public static T[] GetPrefArray<T>(string key, T defaultVal, int arraySize, string fileName)
        {
            Dictionary<string, object> dict = GetPrefDict(fileName);

            T[] tArray = new T[arraySize];
            for (int i = 0; i < arraySize; i++)
            {
                string tempKey = key + i.ToString();
                if (dict.ContainsKey(tempKey))
                    tArray[i] = (T)dict[tempKey];
                else
                    tArray[i] = defaultVal;
            }
            return tArray;
        }

        public static void SetPrefArray<T>(string key, T[] values, string fileName)
        {
            Dictionary<string, object> dict = GetPrefDict(fileName);
            for (int i = 0; i < values.Length; i++)
            {
                dict[key + i.ToString()] = values[i];
            }
            SaveActual(dict, GetPrefsFilename(fileName));
        }

        private static Dictionary<string, object> GetPrefDict(string fileName)
        {
            LoadActual(GetPrefsFilename(fileName), out Dictionary<string, object> dict);
            return dict ?? new Dictionary<string, object>();
        }

        public static void DeleteAllFiles()
        {
            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
            foreach (FileInfo file in info.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in info.GetDirectories())
            {
                dir.Delete(true);
            }
            ExitGame();
        }

        public static void DeleteFile(string fileName, bool quit, bool prefs = false)
        {
            if (prefs)
                fileName = GetPrefsFilename(fileName);
            File.Delete(Application.persistentDataPath + fileName);
            if (quit)
                ExitGame();
        }

        public static string GenerateFilename(string fileMeta, bool unique = true)
        {
            if (!unique) return $"/{fileMeta}.gdat";
            string d = DateTime.Now.ToString();
            d = d.Replace('/', '%');
            d = d.Replace(':', '$');
            fileMeta = $"{fileMeta}-{d}";
            return $"/{fileMeta}.gdat";
        }

        public static string FilenameToDisplay(string fileName, char replacemaentChar)
        {
            fileName = PullMetaFromFilename(fileName);
            fileName = fileName.Substring(1);
            fileName = fileName.Replace('-', replacemaentChar);
            return fileName;
        }

        private static string PullMetaFromFilename(string fileName)
        {
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));
            fileName = fileName.Replace('%', '/');
            fileName = fileName.Replace('$', ':');
            return fileName;
        }
        
        public static void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
#endif