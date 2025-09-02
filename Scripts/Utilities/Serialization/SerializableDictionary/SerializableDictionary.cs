using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Armony.Utilities.Serialization
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        public class KeyWrapper
        {
            public TKey key;

            public KeyWrapper(TKey key)
            {
                this.key = key;
            }
        }

        [SerializeField, HideInInspector]
        private List<KeyWrapper> m_keys = new();

        [SerializeField, HideInInspector]
        private List<TValue> m_values = new();

        public void OnBeforeSerialize()
        {
            if (typeof(TKey).IsEnum)
            {
                m_keys = Enum.GetValues(typeof(TKey)).Cast<TKey>().Select(enumValue => new KeyWrapper(enumValue)).ToList();
            }

            MatchArrayLengths();
        }

        public void OnAfterDeserialize()
        {
            Clear();
            if (typeof(TKey).IsEnum)
            {
                Array enumValues = Enum.GetValues(typeof(TKey));
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if(m_values.Count <= i) return;
                    TKey enumValue = (TKey)enumValues.GetValue(i);
                    Add(enumValue, m_values[i] == null ? default : m_values[i]);
                }
            }
            else
            {
                MatchArrayLengths();
            }
        }

        private void MatchArrayLengths()
        {
            if (m_keys.Count == m_values.Count) return;
            while (m_values.Count < m_keys.Count)
            {
                m_values.Add(default);
            }

            while (m_values.Count > m_keys.Count)
            {
                m_values.RemoveAt(m_values.Count - 1);
            }
        }
    }
}