﻿// A tool for the Unity Editor
// Reverts all the selected gameobjects back to their prefab settings
// How to use
// Select the gameobjects in the hierarchy/scene view
// Navigate to the Tools submenu at the top of the editor
// Click Revert to Prefab

using UnityEditor;
using UnityEngine;

public class RevertAllPrefabsEditor
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
