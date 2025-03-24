using System.Collections.Generic;
using UnityEngine;

namespace Armony.Utilities.Singleton
{
    public static class SingletonManager
    {
        private static Dictionary<System.Type, MonoBehaviour> Singletons { get; } = new();
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
                GameObject singletonObject = new(typeof(T).Name)
                {
                    transform =
                    {
                        parent = GetSingletonHolder()
                    }
                };
                newSingleton = singletonObject.AddComponent<T>();
            }

            Singletons.Add(typeof(T), newSingleton);
            return newSingleton;
        }

        private static Transform GetSingletonHolder()
        {
            if (SingletonHolder) return SingletonHolder;
            SingletonHolder = new GameObject("SingletonHolder").transform;
            Object.DontDestroyOnLoad(SingletonHolder.gameObject);
            return SingletonHolder;
        }
    }
}