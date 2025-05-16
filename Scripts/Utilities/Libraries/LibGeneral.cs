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

        public static string[] GetScenes(int _startIndex = 0)
        {
            int sCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            string[] sList = new string[sCount - _startIndex];
            for (int i = 0; i < sList.Length; i++)
            {
                sList[i] = System.IO.Path.GetFileNameWithoutExtension(
                    UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i + _startIndex)
                );
            }

            return sList;
        }

        //modulus but works with negatives
        public static int LoopInt(this int _i, int _count)
        {
            if (_count > 0) return (_i % _count + _count) % _count;
            Debug.LogError("LoopInt Count less than or equal to 0");
            return 0;

        }

        public static bool IsDigitsOnly(this string _str)
        {
            foreach (char c in _str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        public static void Populate<T>(this T[] _arr, T _value)
        {
            for (int i = 0; i < _arr.Length; ++i)
            {
                _arr[i] = _value;
            }
        }

        public static async void Invoke(Action _f, float _delay = 0f)
        {
            if (_delay > 0)
            {
                await Task.Delay((int)(_delay * 1000));
            }
            else
            {
                await Task.Yield();
            }

            _f();
        }

        public static bool RandomBoolean()
        {
            return UnityEngine.Random.Range(0f, 1f) > 0.5 ? true : false;
        }

        public static int WightedChoice(List<float> _weights)
        {
            float totalWeight = UnityEngine.Random.Range(0, _weights.Sum());

            for (int i = 0; i < _weights.Count; i++)
            {
                totalWeight -= _weights[i];
                if (totalWeight <= 0)
                    return i;
            }

            return _weights.Count - 1;
        }

        public static void DestroyAllChildren(this Transform _parent, bool _immediate = false)
        {
            if (!_immediate)
            {
                foreach (Transform child in _parent)
                {
                    Object.Destroy(child.gameObject);
                }

                return;
            }

            foreach (Transform child in _parent)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }

        public static bool TryGetComponentInParent<T>(this Component _gameObject, out T _parentComponent)
        {
            _parentComponent = _gameObject.GetComponentInParent<T>();
            return _parentComponent != null;
        }

        public static T Random<T>(this IEnumerable<T> _enumerable)
        {
            T[] array = _enumerable.ToArray();
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
    }
}