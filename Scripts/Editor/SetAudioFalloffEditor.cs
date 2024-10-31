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
            ForceSpatialBlendProp = serializedObject.FindProperty("m_forceSpatialBlend");
            SpatialBlendProp = serializedObject.FindProperty("m_spatialBlend");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_forceSpatialBlend", "m_spatialBlend");
            EditorGUILayout.PropertyField(ForceSpatialBlendProp);
            if (ForceSpatialBlendProp.boolValue)
            {
                EditorGUILayout.PropertyField(SpatialBlendProp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    
}