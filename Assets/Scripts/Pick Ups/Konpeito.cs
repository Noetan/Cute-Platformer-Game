// Collectible class
// 

using UnityEngine;
using System.Collections;

public class Konpeito : BasePickUp
{
    #region Inspector
    [SerializeField]
    Material[] ColorSet;
        
    [SerializeField]
    int value = 1;
    #endregion

    // Used to change mesh's material colour
    MeshRenderer m_meshRend;
    // Used to change halo colour
    Light m_light;
    // Used to cycle through the available colours
    static int m_currentColor = 0;

    protected override void Awake()
    {
        m_meshRend = GetComponent<MeshRenderer>();
        m_light = GetComponent<Light>();

        base.Awake();
    }

    protected override void Start()
    {
        CycleColor();

        base.Start();
    }

    protected override void PickUp()
    {
        PlayerController.Instance.AddKonpeito(value);

        base.PickUp();
    }

    // Pick a random color from the ColorSet and assign it
    void RandomizeColor()
    {
        int newColor = Random.Range(0, ColorSet.Length - 1);

        SetColor(newColor);
    }

    // Change the color to the next color in the ColorSet
    // Note this counter is global for all konpeitos in the scene
    void CycleColor()
    {
        int newColor = (m_currentColor++) % ColorSet.Length;
        SetColor(newColor);
    }

    /// <summary>
    /// Changes the colour of the konpeito to one in the ColorSet
    /// </summary>
    /// <param name="newColor">Must be a valid index in the ColorSet</param>
    void SetColor(int newColor)
    {
        if (ColorSet[newColor] == null)
        {
            Debug.LogWarning("Konpeito ColorSet not complete");
            return;
        }

        m_meshRend.material = ColorSet[newColor];
        m_light.color = ColorSet[newColor].color;   // Use this with emissive shader
        //m_light.color = ColorSet[newColor].GetColor("_ReflectionTint");   // Use this is using reflective shader
    }
}