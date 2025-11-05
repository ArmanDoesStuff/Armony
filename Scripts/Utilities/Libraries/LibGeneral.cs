//Copyright AWAN SOFTWORKS LTD 2025

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

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

        public static bool IsDigitsOnly(this string _string)
        {
            return _string.All(_char => _char is >= '0' and <= '9');
        }

        public static async void Invoke(Action _action, float _delay = 0f)
        {
            if (_delay > 0)
            {
                await Task.Delay((int)(_delay * 1000));
            }
            else
            {
                await Task.Yield();
            }

            _action();
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
            int childCount = _parent.childCount;
            if (_immediate)
            {
                for (int i = childCount - 1; i >= 0; i--)
                    Object.DestroyImmediate(_parent.GetChild(i).gameObject);
            }
            else
            {
                for (int i = childCount - 1; i >= 0; i--)
                    Object.Destroy(_parent.GetChild(i).gameObject);
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

        public static T Looped<T>(this IEnumerable<T> _enumerable, int _index)
        {
            T[] array = _enumerable.ToArray();
            return array[_index.Modulus(array.Length)];
        }

        //Same as Array.Fill in .NET Core 2.0+ / .NET 5+
        public static void Populate<T>(this T[] _array, T _value)
        {
            for (int i = 0; i < _array.Length; ++i)
            {
                _array[i] = _value;
            }
        }
    }
}