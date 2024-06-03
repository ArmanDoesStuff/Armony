// From http://forum.unity3d.com/threads/replace-game-object-with-prefab.24311/
// https://bitbucket.org/hsandt/unity-commons-editor/src/develop/ReplaceGameObjects.cs

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Armony.Editor
{
    public class ReplaceGameObjects : EditorWindow
    {
        /// Window vertical scroll position
        private Vector2 m_scrollPosition;

        private GameObject m_replacingObject;
        private bool m_keepSiblingIndex = true;
        private bool m_keepName;
        private bool m_keepIcon;
        private bool m_keepRotation;
        private bool m_keepScale;

        [MenuItem("Tools/Replace Game Objects %g")]
        public static void Init()
        {
            GetWindow<ReplaceGameObjects>(false, "Replace Game Objects", true);
        }

        private void OnGUI()
        {
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

            GUILayout.Label("Use Object", EditorStyles.boldLabel);

            // Expose Replacing Object
            // note that it will be cleared to null when leaving the scene/stage containing it
            m_replacingObject = EditorGUILayout.ObjectField(m_replacingObject, typeof(GameObject), true) as GameObject;

            m_keepSiblingIndex = EditorGUILayout.ToggleLeft("Keep Sibling Index", m_keepSiblingIndex);
            m_keepName = EditorGUILayout.ToggleLeft("Keep Name", m_keepName);
            m_keepIcon = EditorGUILayout.ToggleLeft("Keep Icon", m_keepIcon);
            m_keepRotation = EditorGUILayout.ToggleLeft("Keep Rotation", m_keepRotation);
            m_keepScale = EditorGUILayout.ToggleLeft("Keep Scale", m_keepScale);

            if (GUILayout.Button("Replace Selected"))
            {
                ReplaceSelected();
            }

            EditorGUILayout.EndScrollView();
        }

        private void ReplaceSelected()
        {
            if (m_replacingObject == null) return;
            List<GameObject> replacingObjects = new();

            PrefabStage currentPrefabStage = null;

            // Check current stage
            Stage currentStage = StageUtility.GetCurrentStage();
            if (currentStage is PrefabStage prefabStage)
            {
                // Prefab edit mode
                currentPrefabStage = prefabStage;
            }
            else if (currentStage is not MainStage)
            {
                // Not Prefab edit mode nor Main scene, must be a custom stage
                Debug.LogWarning("Replace Selected is not supported in custom stages");
                return;
            }

            // Selection.transforms, unlike Selection.objects, only keeps the top-most parent
            // if you selected both a parent and a direct or indirect child under it.
            // Since replacing the parent would destroy children anyway, we only need to work with selected roots,
            // so Selection.transforms is what we want to use.

            // However, there is an edge case: Selection.transforms fail to list game objects associated to
            // missing prefabs (shown in red in Hierarchy).
            // So to get them, we check Selection.gameObjects instead, which is exhaustive, but also lists children
            // of other selected objects instead of selection roots only, which we don't want to replace
            // (as explained above).
            // So the trick is to only take selected game objects that are NOT already selected roots or children of selected roots
            // (not that IsChildOf returns true if transforms are equal).
            // Note that this long Linq query is almost equivalent to iterating over all Selection.transforms and
            // RemoveAll game objects that verify IsChildOf, except the enumerable is not interpreted yet,
            // so we will need a ToList conversion later (see remark below).
            IEnumerable<Transform> extraSelectedTransformsToReplaceEnumerable = Selection.gameObjects
                .Where(gameObject => Selection.transforms.All(t => !gameObject.transform.IsChildOf(t)))
                .Select(gameObject => gameObject.transform);

            // Now concatenate both the selected roots and the extra transforms (i.e. the missing prefab objects
            // not already child of selected roots).
            // It is important to resolve the list from the enumerable now, as the enumerable refers to intermediate
            // game objects that may be destroyed in the loop below (when their (indirect) parent is destroyed),
            // causing an error when iterating on them. Whereas the resolved list will exclude all these objects
            // already parented to a selected root, only keeping independent missing prefab objects.
            List<Transform> transformsToReplace = Selection.transforms.Concat(extraSelectedTransformsToReplaceEnumerable).ToList();

            foreach (Transform t in transformsToReplace)
            {
                GameObject replacedGameObject = t.gameObject;

                if (currentPrefabStage != null && replacedGameObject == currentPrefabStage.prefabContentsRoot)
                {
                    Debug.LogErrorFormat("Cannot replace Prefab root {0} in Prefab Edit Mode", replacedGameObject);
                    continue;
                }

                if (PrefabUtility.GetPrefabInstanceStatus(replacedGameObject) == PrefabInstanceStatus.Connected && !PrefabUtility.IsOutermostPrefabInstanceRoot(replacedGameObject))
                {
                    Debug.LogErrorFormat(replacedGameObject, "[ReplaceGameObject] Replace Selected: Cannot delete child {0} of " +
                                                             "prefab instance {1}, please edit the prefab itself.",
                        replacedGameObject, PrefabUtility.GetNearestPrefabInstanceRoot(replacedGameObject));
                    continue;
                }

                // FIXME: also check that we are not in Prefab Edit Mode, with replacing game object being the same
                // prefab as the edited one, or any variant of it, as it would cause cyclic dependency (try to drag it
                // manually and you'll see the error popup)

                GameObject o;

                // check if replacing object is an actual prefab instance root in the Scene (from model, regular or variant prefab)
                // use IsAnyPrefabInstanceRoot to make sure it is a prefab root (including a prefab instance root parented to
                // another prefab instance), and not a non-prefab object parented to a prefab instance
                if (PrefabUtility.GetPrefabInstanceStatus(m_replacingObject) == PrefabInstanceStatus.Connected &&
                    PrefabUtility.IsAnyPrefabInstanceRoot(m_replacingObject))
                {
                    // instantiate it from the prefab to keep the link, but also keep properties
                    // overriden at instance level

                    // Make sure to get the actual prefab for this game object, by using GetPrefabAssetPathOfNearestInstanceRoot.
                    // This will work for both outermost and inner prefab roots.
                    // Other methods like GetCorrespondingObjectFromSource will return the outermost prefab only.
                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(m_replacingObject);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                    // instantiate prefab in same scene as replaced object (this is only needed if object had no
                    // parent, since SetParent below would move the replacing object to the correct scene anyway)
                    o = (GameObject)PrefabUtility.InstantiatePrefab(prefab, replacedGameObject.scene);
                    PrefabUtility.SetPropertyModifications(o, PrefabUtility.GetPropertyModifications(m_replacingObject));
                }

                // check if replacing object is an actual prefab asset from Project view
                // note: we don't check PrefabUtility.GetPrefabAssetType(replacingObject) since a GameObject that
                // is an asset is always a prefab, never PrefabAssetType.NotAPrefab or PrefabAssetType.MissingAsset
                else if (AssetDatabase.Contains(m_replacingObject))
                {
                    // instantiate it with default values, in the same scene as replaced object
                    o = (GameObject)PrefabUtility.InstantiatePrefab(m_replacingObject, replacedGameObject.scene);
                }

                else
                {
                    // replacing object is a non-prefab (not even an instance) or prefab is missing
                    // this includes a non-prefab object located under a prefab root
                    o = Instantiate(m_replacingObject);
                    // the normal Instantiate takes no Scene parameter like InstantiatePrefab, so just move
                    // the replacing object to the right scene manually
                    SceneManager.MoveGameObjectToScene(o, replacedGameObject.scene);
                }

                // NOTES:
                // 1. DuplicateGameObjects calls StageUtility.PlaceGameObjectInCurrentStage(clone)
                // so maybe we should do that to make it work in Prefab Edit Mode, but so far that mode
                // worked without anyway.
                // 2. DuplicateGameObjects only calls SceneManager.MoveGameObjectToScene if
                // currentPrefabStage == null && selectedTransform.parent == null
                // Apparently this doesn't cause an error even when called with replacedGameObject.scene
                // equal to the temporary Prefab Edit scene, but may be cleaner to skip the call
                // if currentPrefabStage != null. And it's not needed if there is a parent too.


                Transform newT = o.transform;

                if (t != null)
                {
                    if (m_keepName)
                        newT.name = t.name;

                    if (m_keepIcon)
                        SetIcon(newT.gameObject, GetIcon(replacedGameObject));

                    // Note that this will fail if object is child of a prefab and was part of the original prefab,
                    // due to prefab hierarchy locking policy. You can check that this object is part of a prefab,
                    // and not the root (see conditions above) if you want to cleanly handle this case.
                    newT.SetParent(t.parent, false);
                    newT.localPosition = t.localPosition;

                    if (m_keepRotation)
                        newT.localRotation = t.localRotation;

                    if (m_keepScale)
                        newT.localScale = t.localScale;

                    if (m_keepSiblingIndex)
                        newT.transform.SetSiblingIndex(t.GetSiblingIndex());

                    replacingObjects.Add(newT.gameObject);
                }

                // Make sure to undo creation after doing all the work on created object properties
                // Usually it works anywhere after creation, but I spot a rare bug where, if
                // Undo.DestroyObjectImmediate fails (it used to fail when trying to replace a game object
                // under some prefab instance root; now we prevent this altogether and return early),
                // Undo-ing the replacing object creation would cause Unity to freeze.
                // And it's cleaner to register creation after complete object setup anyway.
                Undo.RegisterCreatedObjectUndo(o, "Replaced Game Object");

                // Remove original object
                Undo.DestroyObjectImmediate(replacedGameObject);
            }

            // Select new objects
            Selection.objects = replacingObjects.ToArray();
        }

        // based on http://answers.unity3d.com/questions/213140/programmatically-assign-an-editor-icon-to-a-game-o.html
        private static Texture2D GetIcon(GameObject gameObject)
        {
            const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            object[] args = new object[] { gameObject };
            MethodInfo setIcon = typeof(EditorGUIUtility).GetMethod("GetIconForObject", flags);
            return setIcon?.Invoke(null, args) as Texture2D;
        }

        private static void SetIcon(GameObject gameObject, Texture2D icon)
        {
            const BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            object[] args = new object[] { gameObject, icon };
            MethodInfo setIcon = typeof(EditorGUIUtility).GetMethod("SetIconForObject", flags);
            setIcon?.Invoke(null, args);
        }
    }
}