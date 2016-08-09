// Just adds a note explaining how the debug sphere is drawn

using UnityEngine;
using UnityEditor;
using UnityChan;

[CustomEditor(typeof(SpringCollider))]
[CanEditMultipleObjects]
public class SpringColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(5);

        GUILayout.TextArea("Note: debug sphere scaled with the world scale.x");
    }
}
