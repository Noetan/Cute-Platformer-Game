// Adds a button to the SpringManager's component panel

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
            SpringBone[] bones = sm.gameObject.GetComponentsInChildren<SpringBone>();
            sm.springBones = bones;
        }
    }
}
