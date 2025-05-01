using Armony.Scripts.Utilities.Audio;

namespace Armony.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(SetAudioFalloff))]
    public class SetAudioFalloffEditor : Editor
    {
        SerializedProperty ForceSpatialBlendProp{ get; set; }
        private SerializedProperty SpatialBlendProp { get; set; }

        private void OnEnable()
        {
            ForceSpatialBlendProp = serializedObject.FindProperty("forceSpatialBlend");
            SpatialBlendProp = serializedObject.FindProperty("spatialBlend");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "forceSpatialBlend", "spatialBlend");
            EditorGUILayout.PropertyField(ForceSpatialBlendProp);
            if (ForceSpatialBlendProp.boolValue)
            {
                EditorGUILayout.PropertyField(SpatialBlendProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    
}