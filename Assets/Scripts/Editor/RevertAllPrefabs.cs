using UnityEditor;
using UnityEngine;

public class RevertAllPrefabs
{
    [MenuItem("Tools/Revert to Prefab")]
    static void Revert()
    {
        var selection = Selection.gameObjects;

        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                PrefabUtility.ResetToPrefabState(selection[i]);
            }
        }
        else
        {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }
}
