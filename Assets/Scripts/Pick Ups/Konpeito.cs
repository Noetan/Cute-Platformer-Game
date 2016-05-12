// Collectible class
// 

using UnityEngine;
using UnityEngine.Assertions;

public class Konpeito : BasePickUp
{
    #region Inspector
    [Header("Konpeito Graphics")]
    [SerializeField]
    Material[] ColorSet;

    [Header("Konpeito gameplay")]
    // Possible values for the konpeito to be worth
    [SerializeField]
    int[] values = { 1, 2, 5, 10, 15, 20 };
    #endregion
    
    // Used to cycle through the available colours
    static int m_currentColor = 0;
    // cache propety id of konpeito's material's shader's color
    int m_trailShaderColorID = -1;

    protected override void Awake()
    {
        base.Awake();
        
        m_trailRender = GetComponent<TrailRenderer>();
        
        Assert.IsNotNull(m_trailRender);

        m_trailShaderColorID = Shader.PropertyToID("_TintColor");         
        CycleColor();
    }

    protected override void PickUp()
    {
        // Increase the player's moneys
        if (values.Length > 0)
        {
            PlayerController.Instance.AddKonpeito(values[Random.Range(0, values.Length)]);
        }
        else
        {
            PlayerController.Instance.AddKonpeito(1);
            Debug.LogWarning("konpeito values is not set and empty");
        }

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

        m_trailRender.material.SetColor(m_trailShaderColorID, ColorSet[newColor].color);
    }
}