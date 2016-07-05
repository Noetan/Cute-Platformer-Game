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

        // TODO: Find all SpringBones in children in depth first order
        // Create array of size numChildren
        // set array
        EditorGUILayout.HelpBox("TODO: Automatically assign springbones -Lemon", MessageType.None, false);

        if (GUILayout.Button("Find Spring Bones"))
        {
            // Foo
            SpringBone[] bones = sm.gameObject.GetComponentsInChildren<SpringBone>();
            sm.springBones = bones;
        }
    }
}
