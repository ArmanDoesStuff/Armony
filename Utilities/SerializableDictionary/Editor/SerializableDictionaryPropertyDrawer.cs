using System.Collections.Generic;
using Mono.CSharp;
using UnityEditor;
using UnityEngine;

namespace Armony.Utilities.SerializableDictionary
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty keysProperty = property.FindPropertyRelative("m_keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("m_values");
            bool keyIsEnum = keysProperty.arraySize > 0 && keysProperty.GetArrayElementAtIndex(0).FindPropertyRelative("key").propertyType == SerializedPropertyType.Enum;
            EditorGUILayout.PrefixLabel(label);

            if (!IsDictionaryValid(keysProperty))
            {
                GUIStyle errorStyle = new(GUI.skin.label) { normal = { textColor = Color.red } };
                EditorGUILayout.LabelField("Dictionary is invalid - Duplicate keys will be ignored", errorStyle);
            }

            EditorGUI.indentLevel++;
            int keyCount = 0;
            if (!keyIsEnum)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Key Count: ", GUILayout.Width(EditorGUIUtility.currentViewWidth * .2f - EditorGUIUtility.fieldWidth));
                EditorGUI.BeginChangeCheck();
                keyCount = EditorGUILayout.IntField(keysProperty.arraySize, GUILayout.Width(EditorGUIUtility.fieldWidth));
                EditorGUILayout.EndHorizontal();
            }

            float layoutWidth = EditorGUIUtility.currentViewWidth - EditorGUIUtility.standardVerticalSpacing;
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                bool keyIsValid = keysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("isValid").boolValue;
                GUI.backgroundColor = keyIsValid ? GUI.skin.box.normal.textColor : Color.red;
                
                EditorGUILayout.BeginHorizontal();
                SerializedProperty key = keysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                if (keyIsEnum)
                    EditorGUILayout.LabelField(key.enumNames[key.enumValueIndex], GUILayout.Width(layoutWidth * .33f));
                else
                    EditorGUILayout.PropertyField(key, GUIContent.none, GUILayout.Width(layoutWidth * .33f));
                EditorGUILayout.PropertyField(valuesProperty.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(layoutWidth * .66f));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;

            if (!keyIsEnum && EditorGUI.EndChangeCheck())
            {
                keysProperty.arraySize = keyCount;
            }

            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
            EditorGUI.EndProperty();
        }

        private bool IsDictionaryValid(SerializedProperty keysProperty)
        {
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                SerializedProperty wrapperProperty = keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty isValidProperty = wrapperProperty.FindPropertyRelative("isValid");
                if (!isValidProperty.boolValue)
                {
                    return false;
                }
            }

            return true;
        }
    }
}