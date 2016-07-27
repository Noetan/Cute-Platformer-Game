// Adds a button to the SpringManager's component panel

using UnityEngine;
using UnityEditor;
using UnityChan;

[CustomEditor(typeof(SpringBone))]
public class SpringBoneEditor : Editor
{
    Transform[] children;
    SpringBone sb;
    int currentChild = 1;

    public override void OnInspectorGUI()
    {
        sb = (SpringBone)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Find Next Child"))
        {
            sb.child = GetNextChild();
        }
    }

    Transform GetNextChild()
    {
        if (children == null)
        {
            children = sb.GetComponentsInChildren<Transform>(false);
        }
        
        return children[(currentChild++ % children.Length)];
    }
}
