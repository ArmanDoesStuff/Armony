using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Armony.Utilities.Serialization.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty _property)
        {
            VisualElement root = new()
            {
                style =
                {
                    flexDirection = FlexDirection.Column
                }
            };

            Foldout foldout = new() { text = _property.displayName, value = true };
            root.Add(foldout);

            SerializedProperty keysProperty = _property.FindPropertyRelative("m_keys");
            SerializedProperty valuesProperty = _property.FindPropertyRelative("m_values");

            bool keyIsEnum = keysProperty.arraySize > 0 &&
                             keysProperty.GetArrayElementAtIndex(0).FindPropertyRelative("key").propertyType ==
                             SerializedPropertyType.Enum;

            // Optional: show key count if not enum
            if (!keyIsEnum)
            {
                IntegerField countField = new("Key Count") { value = keysProperty.arraySize };
                countField.RegisterValueChangedCallback(_changeEvent =>
                {
                    int newSize = Mathf.Max(0, _changeEvent.newValue);
                    keysProperty.arraySize = newSize;
                    valuesProperty.arraySize = newSize;
                    _property.serializedObject.ApplyModifiedProperties();
                    foldout.schedule.Execute(() =>
                    {
                        SerializedObject so = _property.serializedObject;
                        RebuildDictionaryUI(foldout, keysProperty, valuesProperty, false);
                        foldout.Bind(so);
                    }).ExecuteLater(0);
                });
                foldout.Add(countField);
            }

            RebuildDictionaryUI(foldout, keysProperty, valuesProperty, keyIsEnum);
            return root;
        }

        private void RebuildDictionaryUI(VisualElement _foldout, SerializedProperty _keysProperty,
            SerializedProperty _valuesProperty, bool _keyIsEnum)
        {
            while (_foldout.childCount > (_keyIsEnum ? 0 : 1))
                _foldout.RemoveAt(_foldout.childCount - 1);

            HashSet<int> duplicateIndices = new();

            if (!_keyIsEnum)
            {
                Dictionary<string, List<int>> keyToIndices = new();

                for (int i = 0; i < _keysProperty.arraySize; i++)
                {
                    SerializedProperty keyWrapper = _keysProperty.GetArrayElementAtIndex(i);
                    SerializedProperty keyProp = keyWrapper.FindPropertyRelative("key");

                    string keyString = GetKeyString(keyProp);

                    if (!keyToIndices.ContainsKey(keyString))
                        keyToIndices[keyString] = new List<int>();

                    keyToIndices[keyString].Add(i);
                }

                foreach (int index in keyToIndices.Where(_kvp => _kvp.Value.Count > 1).SelectMany(_kvp => _kvp.Value))
                {
                    duplicateIndices.Add(index);
                }
            }

            for (int i = 0; i < _keysProperty.arraySize; i++)
            {
                VisualElement row = new() { style = { flexDirection = FlexDirection.Row } };

                SerializedProperty keyWrapper = _keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty keyProp = keyWrapper.FindPropertyRelative("key");
                SerializedProperty valueProp = _valuesProperty.GetArrayElementAtIndex(i);

                if (_keyIsEnum)
                {
                    string enumLabel = ObjectNames.NicifyVariableName(keyProp.enumNames[keyProp.enumValueIndex]);
                    row.Add(new Label(enumLabel)
                    {
                        style =
                        {
                            flexBasis = Length.Percent(30),
                            unityTextAlign = TextAnchor.MiddleLeft
                        }
                    });
                }
                else
                {
                    PropertyField keyField = new(keyProp, "") { style = { flexBasis = Length.Percent(30) } };

                    if (duplicateIndices.Contains(i))
                    {
                        keyField.RegisterCallback<GeometryChangedEvent>(_ =>
                        {
                            var input = keyField.Q<VisualElement>("unity-text-input"); // works for most input types
                            if (input != null)
                            {
                                input.style.borderBottomColor = Color.red;
                                input.style.borderTopColor = Color.red;
                                input.style.borderLeftColor = Color.red;
                                input.style.borderRightColor = Color.red;
                            }
                        });
                    }

                    row.Add(keyField);

                    keyField.RegisterCallback<FocusOutEvent>(_ =>
                    {
                        _keysProperty.serializedObject.ApplyModifiedProperties();
                        _foldout.schedule.Execute(() =>
                        {
                            SerializedObject so = _keysProperty.serializedObject;
                            RebuildDictionaryUI(_foldout, _keysProperty, _valuesProperty, false);
                            _foldout.Bind(so);
                        }).ExecuteLater(0);
                    });
                }

                PropertyField valueField = new(valueProp, "") { style = { flexGrow = 1 } };
                row.Add(valueField);

                _foldout.Add(row);
            }

            if (duplicateIndices.Count <= 0) return;
            HelpBox warning = new("Duplicate keys detected!", HelpBoxMessageType.Warning);
            _foldout.Add(warning);
        }

        private string GetKeyString(SerializedProperty _keyProp)
        {
            return _keyProp.propertyType switch
            {
                SerializedPropertyType.String => _keyProp.stringValue,
                SerializedPropertyType.Integer => _keyProp.intValue.ToString(),
                SerializedPropertyType.Enum => _keyProp.enumNames[_keyProp.enumValueIndex],
                SerializedPropertyType.ObjectReference => _keyProp.objectReferenceValue
                    ? _keyProp.objectReferenceValue.name
                    : "null",
                _ => _keyProp.ToString()
            };
        }
    }
}