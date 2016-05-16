// Lists the enums in the PooledDB class into the inspector
// So the user can easily assign prefabs in the right order

using UnityEditor;
using System;

[CustomEditor(typeof(AudioPool))]
public class AudioPoolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        string helpBox = "";
        string clipString = "";

        for (int i = 0; i < Helper.CountEnum(typeof(AudioPool.MixerGroup)); i++)
        {
            clipString = clipString + "\n" + i + ". " + Enum.GetName(typeof(AudioPool.MixerGroup), i);
        }

        helpBox = string.Format("   MixerGroup ({0}) \n{1}",
            Helper.CountEnum(typeof(AudioPool.MixerGroup)),
            clipString);

        EditorGUILayout.HelpBox(helpBox, MessageType.None, false);
        
        DrawDefaultInspector();        
    }
}
