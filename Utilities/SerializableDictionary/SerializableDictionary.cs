using System;
using System.Collections.Generic;
using UnityEngine;

namespace Armony.Utilities.SerializableDictionary
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> m_keys = new();

        [SerializeField]
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
                    m_keys.Add(enumValue);
                    m_values.Add(TryGetValue(enumValue, out TValue value) ? value : default);
                }
                return;
            }
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
                for (int i = 0; i < m_keys.Count; i++)
                {
                    if (i < m_values.Count)
                    {
                        TryAdd(m_keys[i], m_values[i]);
                    }
                }
            }
        }
    }
}