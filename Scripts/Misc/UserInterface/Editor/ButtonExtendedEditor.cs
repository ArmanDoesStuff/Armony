using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Armony.Misc.UserInterface.Editor
{
    [CustomEditor(typeof(ButtonExtended))]
    public class ButtonExtendedEditor : ButtonEditor
    {
        private SerializedProperty[] m_serializedProperties;

        private IEnumerable<FieldInfo> GetAllFields()
        {
            List<FieldInfo> fieldsList = new();
            Type type = target.GetType();

            while (type != typeof(ButtonExtended).BaseType)
            {
                // ReSharper disable once PossibleNullReferenceException
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                fieldsList.InsertRange(0, fields);
                type = type.BaseType;
            }

            return fieldsList.ToArray();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            serializedObject.Update();

            m_serializedProperties = (from field in GetAllFields()
                where field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), true).Length > 0
                select serializedObject.FindProperty(field.Name)
                into serializedProperty
                where serializedProperty != null
                select serializedProperty).ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            foreach (SerializedProperty property in m_serializedProperties)
            {
                EditorGUILayout.PropertyField(property);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}