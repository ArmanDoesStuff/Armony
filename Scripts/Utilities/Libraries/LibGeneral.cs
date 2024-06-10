//AWAN SOFTWORKS LTD 2023

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Armony.Utilities.Libraries
{
    public static partial class LibGeneral
    {
        public static void QuitGame()
        {
#if UNITYEDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static string[] GetScenes(int startIndex = 0)
        {
            int sCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            string[] sList = new string[sCount - startIndex];
            for (int i = 0; i < sList.Length; i++)
            {
                sList[i] = System.IO.Path.GetFileNameWithoutExtension(
                    UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i + startIndex)
                );
            }
            return sList;
        }

        public static int LoopInt(this int i, int count)
        {
            if (count <= 0)
            {
                Debug.LogError("LoopInt Count less than or equal to 0");
                return 0;
            }

            while (i < 0)
            {
                i += count;
            }
            return i % count;
        }

        public static bool IsDigitsOnly(this string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i] = value;
            }
        }

        public static async void Invoke(Action f, float delay = 0f)
        {
            if (delay > 0)
            {
                await Task.Delay((int)(delay * 1000));
            }
            else
            {
                await Task.Yield();
            }
            f();
        }

        public static bool RandomBoolean()
        {
            return UnityEngine.Random.Range(0f, 1f) > 0.5 ? true : false;
        }

        public static int WightedChoice(List<float> weights)
        {
            float totalWeight = UnityEngine.Random.Range(0, weights.Sum());

            for (int i = 0; i < weights.Count; i++)
            {
                totalWeight -= weights[i];
                if (totalWeight <= 0)
                    return i;
            }
            return weights.Count - 1;
        }

        public static void ClearGameObjects<T>(ref List<T> gameObjectsList)
        where T : MonoBehaviour
        {
            foreach (T obj in gameObjectsList)
            {
                Object.Destroy(obj.gameObject);
            }
            gameObjectsList.Clear();
        }
        public static void DestroyAllChildren(this Transform parent)
        {
            foreach (Transform child in parent)
            {
                Object.Destroy(child.gameObject);
            }
        }
        
        public static bool TryGetComponentInParent<T>(this Component gameObject, out T parentComponent)
        {
            parentComponent = gameObject.GetComponentInParent<T>();
            return parentComponent != null;
        }
        
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            T[] array = enumerable.ToArray();
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
    }
}
