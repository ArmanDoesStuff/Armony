using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Armony.Utilities.SerializableDictionary.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        private bool ShowProperty { get; set; } = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect foldoutPosition = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            ShowProperty = EditorGUI.Foldout(foldoutPosition, ShowProperty, label);
            position.y += EditorGUIUtility.singleLineHeight;
            if (!ShowProperty) return;
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty keysProperty = property.FindPropertyRelative("m_keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("m_values");
            bool keyIsEnum = keysProperty.arraySize > 0 && keysProperty.GetArrayElementAtIndex(0).FindPropertyRelative("key").propertyType == SerializedPropertyType.Enum;

            EditorGUI.indentLevel++;
            int keyCount = 0;
            if (!keyIsEnum)
            {
                Rect keyRect = new(position.x, position.y, EditorGUIUtility.currentViewWidth * .2f - EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(keyRect, "Key Count:");

                keyRect = new(position.x + keyRect.width, position.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginChangeCheck();
                keyCount = EditorGUI.IntField(keyRect, keysProperty.arraySize);

                if (!IsDictionaryValid(keysProperty))
                {
                    keyRect.x += EditorGUIUtility.fieldWidth;
                    keyRect.width = EditorGUIUtility.currentViewWidth - keyRect.width;
                    EditorGUI.LabelField(keyRect, "Dictionary is invalid - Duplicate keys will be ignored");
                }

                position.y += EditorGUIUtility.singleLineHeight;
            }

            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                bool keyIsValid = keysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("isValid").boolValue;
                GUI.backgroundColor = keyIsValid ? GUI.skin.box.normal.textColor : Color.red;

                SerializedProperty key = keysProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key");

                Rect keyRect = new(position.x, position.y, position.width * 0.3f, EditorGUIUtility.singleLineHeight);
                Rect valueRect = new(position.x + position.width * 0.3f, position.y, position.width * 0.7f, EditorGUIUtility.singleLineHeight);
                if (keyIsEnum)
                {
                    EditorGUI.LabelField(keyRect, key.enumNames[key.enumValueIndex]);
                }
                else
                {
                    EditorGUI.PropertyField(keyRect, key, GUIContent.none);
                }

                EditorGUI.PropertyField(valueRect, valuesProperty.GetArrayElementAtIndex(i), GUIContent.none);
                position.y += EditorGUIUtility.singleLineHeight;
            }

            EditorGUI.indentLevel--;

            if (!keyIsEnum && EditorGUI.EndChangeCheck())
            {
                keysProperty.arraySize = keyCount;
            }

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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty keysProperty = property.FindPropertyRelative("m_keys");
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            if (ShowProperty)
            {
                height += EditorGUIUtility.singleLineHeight * keysProperty.arraySize;
            }

            return height;
        }
    }
}