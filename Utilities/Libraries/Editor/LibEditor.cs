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
    }
}
