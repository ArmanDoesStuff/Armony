using UnityEngine;
using UnityEditor;

namespace Armony.Editor
{
    public abstract class CustomMenuItems
    {
        /// <summary>
        /// Implement via MenuItem: [MenuItem("GameObject/{GameName}/{PrefabName}")]
        /// </summary>
        protected static void SpawnCustomPrefab(string prefabPath, MenuCommand menuCommand)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab != null)
            {
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, ((GameObject)menuCommand.context).transform);
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
    }
}