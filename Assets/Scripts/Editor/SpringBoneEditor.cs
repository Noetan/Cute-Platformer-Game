// Adds a button to automatically assign the child bone

using UnityEngine;
using UnityEditor;
using UnityChan;
using UnityEngine.Assertions;

[CustomEditor(typeof(SpringBone))]
[CanEditMultipleObjects]
public class SpringBoneEditor : Editor
{
    Transform[] children;
    SpringBone sb;
    int currentChild = 1; // 1 because 0 is itself and not the first child

    public override void OnInspectorGUI()
    {
        sb = (SpringBone)target;

        DrawDefaultInspector();

        GUILayout.Space(10);

        if (GUILayout.Button("Find First Child"))
        {
            sb.child = GetNextChild(false);
        }
        if (GUILayout.Button("Find Next Child"))
        {
            sb.child = GetNextChild(true);
        }

        GUILayout.Space(5);

        GUILayout.TextArea("WARNING: Uses world scale.x to scale the bones and colliders.\nDO NOT SCALE THE GAMEOBJECT NON-UNIFORMLY (Very bad idea with rigidbodies anyway).");
    }

    Transform GetNextChild(bool cycle)
    {
        children = sb.GetComponentsInChildren<Transform>(false);

        Assert.IsTrue(children.Length > 0);
		
        if (cycle)
        {
            return children[(currentChild++ % children.Length)];
        }
        else
        {
            return children[1];
        }
    }
}
