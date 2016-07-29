// Adds a button to the SpringManager's component panel to automatically find SpringBones

using UnityEngine;
using UnityEditor;
using UnityChan;

[CustomEditor(typeof(SpringManager))]
public class SpringManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpringManager sm = (SpringManager)target;

        DrawDefaultInspector();
                
        if (GUILayout.Button("Find Spring Bones"))
        {
            sm.springBones = sm.gameObject.GetComponentsInChildren<SpringBone>(false);
        }
    }
}
