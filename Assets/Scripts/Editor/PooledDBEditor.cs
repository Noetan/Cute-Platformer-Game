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

        helpBox =
            "Particles\n" + particleEnum + "\n\nPick Ups\n" + pickupEnum;

        EditorGUILayout.HelpBox(helpBox, MessageType.None, false);

        DrawDefaultInspector();
    }
}
