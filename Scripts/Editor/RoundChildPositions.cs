using UnityEngine;
using UnityEditor;

public class RoundChildPositions : EditorWindow
{
    private GameObject parentObject;
    private int roundTo = 5;

    [MenuItem("Tools/Round Child Positions")]
    public static void ShowWindow()
    {
        GetWindow<RoundChildPositions>("Round Child Positions");
    }

    private void OnGUI()
    {
        GUILayout.Label("Round Child Positions", EditorStyles.boldLabel);
        GUILayout.Space(5);

        parentObject = (GameObject)EditorGUILayout.ObjectField(
            "Parent Object", parentObject, typeof(GameObject), true);

        roundTo = EditorGUILayout.IntField("Round To Nearest", roundTo);

        GUILayout.Space(10);

        GUI.enabled = parentObject != null;
        if (GUILayout.Button("Round Positions"))
        {
            RoundPositions();
        }
        GUI.enabled = true;

        if (parentObject == null)
        {
            EditorGUILayout.HelpBox("Assign a parent GameObject to get started.", MessageType.Info);
        }
    }

    private void RoundPositions()
    {
        Undo.SetCurrentGroupName("Round Child Positions");
        int undoGroup = Undo.GetCurrentGroup();

        int count = 0;
        foreach (Transform child in parentObject.transform)
        {
            Undo.RecordObject(child, "Round Position");
            Vector3 pos = child.localPosition;
            child.localPosition = new Vector3(
                RoundToNearest(pos.x, roundTo),
                RoundToNearest(pos.y, roundTo),
                RoundToNearest(pos.z, roundTo)
            );
            count++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log($"[RoundChildPositions] Rounded {count} child(ren) to nearest {roundTo}.");
    }

    private float RoundToNearest(float value, float nearest)
    {
        return Mathf.Round(value / nearest) * nearest;
    }
}