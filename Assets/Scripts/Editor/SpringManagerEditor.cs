// Adds a button to the SpringManager's component panel

using UnityEditor;
using UnityChan;

[CustomEditor(typeof(SpringManager))]
public class SpringManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // TODO: Find all SpringBones in children in depth first order
        // Create array of size numChildren
        // set array
        EditorGUILayout.HelpBox("TODO: Automatically assign springbones -Lemon", MessageType.None, false);
    }
}
