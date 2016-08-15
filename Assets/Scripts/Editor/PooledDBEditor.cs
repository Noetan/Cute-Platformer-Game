// Lists the enums in the PooledDB class into the inspector
// So the user can easily assign prefabs in the right order

using UnityEditor;
using System;

[CustomEditor( typeof(PooledDB) )]
public class PooledDBEditor : Editor
{
    public override void OnInspectorGUI()
    {
        string helpBox = "";
        string particleEnum = "";
        string pickupEnum = "";
        
        for (int i = 0; i < Helper.CountEnum(typeof(Pools.Particles)); i++)
        {
            particleEnum = particleEnum + "\n" + i + ". " + Enum.GetName(typeof(Pools.Particles), i);
        }

        for (int i = 0; i < Helper.CountEnum(typeof(Pools.PickUps)); i++)
        {
            pickupEnum = pickupEnum + "\n" + i + ". " + Enum.GetName(typeof(Pools.PickUps), i);
        }

        helpBox = string.Format("  Particles ({0}) \n{1} \n\n  Pick Ups ({2}) \n{3}", 
            Helper.CountEnum(typeof(Pools.Particles)),
            particleEnum,
            Helper.CountEnum(typeof(Pools.PickUps)),
            pickupEnum );

        EditorGUILayout.HelpBox(helpBox, MessageType.None, false);
        
        DrawDefaultInspector();
    }
}
