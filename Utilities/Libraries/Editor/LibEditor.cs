using UnityEditor;
using UnityEngine;

namespace Armony.Utilities.Libraries.Editor
{
    public static class LibEditor
    {
        [MenuItem("Arman Library/Anchors to Corners %[")]
        private static void Shortcut_AnchorToCorner()
        {
            foreach (Transform t in Selection.transforms)
            {
                RectTransform rectTransform = t.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.AnchorToCorner();
                }
            }
        }

        [MenuItem("Arman Library/Corners to Anchors %]")]
        private static void Shortcut_CornerToAnchor()
        {
            foreach (Transform t in Selection.transforms)
            {
                RectTransform rectTransform = t.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.CornerToAnchor();
                }
            }
        }
        
        [MenuItem("Assets/Copy GUID")]
        private static void CopySelectedAssetGUID()
        {
            Object selectedObject = Selection.activeObject;
            if (selectedObject == null)
            {
                Debug.LogWarning("No asset selected.");
                return;
            }

            string assetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(selectedObject));
            if (string.IsNullOrEmpty(assetGUID))
            {
                Debug.LogWarning("Failed to retrieve GUID for selected asset.");
                return;
            }

            // Copy the GUID to the clipboard
            EditorGUIUtility.systemCopyBuffer = assetGUID;
            Debug.Log("Copied asset GUID to clipboard: " + assetGUID);
        }
    }
}
