using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Armony.Editor
{
    public abstract class CustomMenuItems
    {
        /// <summary>
        /// Implement via MenuItem: [MenuItem("GameObject/{GameName}/{PrefabName}")]
        /// </summary>
        protected static void SpawnPrefabInScene(GUID guid, MenuCommand menuCommand)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            if (prefab != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, ((GameObject)menuCommand.context)?.transform);
                if (instance != null)
                {
                    PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                    Selection.activeGameObject = instance;
                }
                else
                {
                    Debug.LogError("Failed to instantiate prefab.");
                }
            }
            else
            {
                Debug.LogError("Prefab not found. Make sure it's located in a Resources folder.");
            }
        }

        public static async void CreatePrefab(GUID guid, MenuCommand menuCommand)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab != null)
            {
                string selectedFolderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                string prefabName = prefabPath.Substring(prefabPath.LastIndexOf('/'));
                string newPrefabPath = AssetDatabase.GenerateUniqueAssetPath(selectedFolderPath + prefabName);
                AssetDatabase.CopyAsset(prefabPath, newPrefabPath);
                AssetDatabase.Refresh();
                GameObject prefabInstance = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);
                Selection.activeObject = prefabInstance;
                await Task.Delay(10);
                EditorWindow.focusedWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
            }
            else
            {
                Debug.LogError("Prefab not found at path: " + prefabPath);
            }
        }
    }
}