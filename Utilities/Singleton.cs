using UnityEngine;

namespace Armony.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance;
        public static T Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance ??= FindAnyObjectByType<T>();
                if (s_instance != null) return s_instance;
                GameObject singletonObject = new();
                s_instance = singletonObject.AddComponent<T>();
                singletonObject.name = typeof(T).ToString();

                DontDestroyOnLoad(singletonObject);
                return s_instance;
            }
        }

        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}