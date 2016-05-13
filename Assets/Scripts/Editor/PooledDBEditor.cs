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

        for (int i = 0; i < Helper.CountEnum(typeof(PooledDB.Particle)); i++)
        {
            particleEnum = particleEnum + "\n" + Enum.GetName(typeof(PooledDB.Particle), i);
        }

        for (int i = 0; i < Helper.CountEnum(typeof(PooledDB.PickUp)); i++)
        {
            pickupEnum = pickupEnum + "\n" + Enum.GetName(typeof(PooledDB.PickUp), i);
        }

        helpBox = string.Format("Particles ({0}) \n{1} \n\nPick Ups ({2}) \n{3}", 
            Helper.CountEnum(typeof(PooledDB.Particle)),
            particleEnum,
            Helper.CountEnum(typeof(PooledDB.PickUp)),
            pickupEnum );

        EditorGUILayout.HelpBox(helpBox, MessageType.None, false);

        DrawDefaultInspector();
    }
}
