using System.Collections.Generic;
using UnityEngine;

namespace Armony.Utilities.Singleton
{
    public static class SingletonManager
    {
        private static Dictionary<System.Type, MonoBehaviour> Singletons { get; set; } = new();
        private static Transform SingletonHolder { get; set; }

        public static T GetInstance<T>()
            where T : MonoBehaviour
        {
            if (Singletons.TryGetValue(typeof(T), out MonoBehaviour singleton))
            {
                return (T)singleton;
            }

            T newSingleton = Object.FindAnyObjectByType<T>();
            if (newSingleton == null)
            {
                GameObject singletonObject = new();
                newSingleton = singletonObject.AddComponent<T>();
                singletonObject.name = typeof(T).ToString();
                singletonObject.transform.parent = GetSingletonHolder();
            }

            Singletons.Add(typeof(T), newSingleton);
            return newSingleton;
        }

        private static Transform GetSingletonHolder()
        {
            if (!SingletonHolder)
            {
                SingletonHolder = new GameObject("SingletonHolder").transform;
                Object.DontDestroyOnLoad(SingletonHolder.gameObject);
            }

            return SingletonHolder;
        }
    }
}