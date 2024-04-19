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
            bool keyIsEnum = keysProperty.arraySize > 0 && keysProperty.GetArrayElementAtIndex(0).propertyType == SerializedPropertyType.Enum;
            EditorGUILayout.PrefixLabel(label);
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

            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(keysProperty.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(EditorGUIUtility.currentViewWidth * .33f - EditorGUIUtility.standardVerticalSpacing));
                EditorGUILayout.PropertyField(valuesProperty.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(EditorGUIUtility.currentViewWidth * .66f - EditorGUIUtility.standardVerticalSpacing));
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

        private static float GetHeight(SerializedProperty property)
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;
            if (!property.FindPropertyRelative("m_keys").isExpanded && !property.FindPropertyRelative("m_values").isExpanded) return totalHeight;
            SerializedProperty keys = property.FindPropertyRelative("m_keys");
            totalHeight += EditorGUIUtility.singleLineHeight * (keys.arraySize + 2);
            return totalHeight;
        }
    }
}