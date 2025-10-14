using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectionPercent : EditorWindow
{
    [Range(0f, 1f)] float selectionPercent = 0.2f;

    [MenuItem("Tools/Selection Percent")]
    public static void ShowWindow()
    {
        GetWindow<SelectionPercent>("Selection Percent");
    }

    void OnGUI()
    {
        GUILayout.Label("Random Selection from Current", EditorStyles.boldLabel);
        selectionPercent = EditorGUILayout.Slider("Selection Percent", selectionPercent, 0f, 1f);

        if (GUILayout.Button("Select Random Subset"))
        {
            SelectRandomSubset();
        }
    }

    void SelectRandomSubset()
    {
        Object[] currentSelection = Selection.objects;
        if (currentSelection.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "You must select some objects in the Hierarchy first.", "OK");
            return;
        }

        int countToSelect = Mathf.FloorToInt(currentSelection.Length * selectionPercent);
        HashSet<int> selectedIndices = new HashSet<int>();
        while (selectedIndices.Count < countToSelect)
            selectedIndices.Add(Random.Range(0, currentSelection.Length));

        Object[] newSelection = selectedIndices.Select(i => currentSelection[i]).ToArray();
        Selection.objects = newSelection;

        Debug.Log($"Selected {newSelection.Length} of {currentSelection.Length} objects.");
    }
}