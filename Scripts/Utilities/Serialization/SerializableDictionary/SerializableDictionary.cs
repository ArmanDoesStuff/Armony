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
            public bool isValid;

            public KeyWrapper(TKey key, bool isValid = true)
            {
                this.key = key;
                this.isValid = isValid;
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
                m_keys.Clear();
                m_values.Clear();
                Array enumValues = Enum.GetValues(typeof(TKey));
                for (int i = 0; i < enumValues.Length; i++)
                {
                    TKey enumValue = (TKey)enumValues.GetValue(i);
                    m_keys.Add(new KeyWrapper(enumValue));
                    m_values.Add(TryGetValue(enumValue, out TValue value) ? value : default);
                }

                return;
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
                    TKey enumValue = (TKey)enumValues.GetValue(i);
                    Add(enumValue, m_values[i] == null ? default : m_values[i]);
                }
            }
            else
            {
                MatchArrayLengths();
                for (int i = 0; i < m_keys.Count; i++)
                {
                    m_keys[i].isValid = TryAdd(m_keys[i].key, m_values[i]);
                }
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