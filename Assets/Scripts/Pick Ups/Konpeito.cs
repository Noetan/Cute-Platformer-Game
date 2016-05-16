// Collectible class
// 

using UnityEngine;
using UnityEngine.Assertions;

public class Konpeito : BasePickUp
{
    #region Inspector
    [Header("Konpeito Graphics")]
    [SerializeField]
    Material[] m_colorSet;

    [Header("Konpeito gameplay")]
    // Possible values for the konpeito to be worth
    [SerializeField]
    int[] m_values = { 1, 2, 5, 10, 15, 20 };
    [SerializeField]
    bool m_massSpawned = false;
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
    static float diff = -12.0f;
    protected override void PickUp()
    {
        // Increase the player's moneys
        if (m_values.Length > 0)
        {
            PlayerController.Instance.AddKonpeito(m_values[Random.Range(0, m_values.Length)]);
        }
        else
        {
            PlayerController.Instance.AddKonpeito(1);
            Debug.LogWarning("konpeito values is not set and empty");
        }

        // Disable the item
        CurrentState = State.Disabled;
        ShowModel(false);
        
        Debug.Log(diff);
        if (m_massSpawned)
        {
            AudioPool.Instance.PlayRandom(m_touchedSFX, transform.position, m_SFXSettings);
        }
        else
        {
            AudioPool.Instance.PlayPitchShift(m_touchedSFX, transform.position, m_SFXSettings, diff++, "CollectiblesPitchShift");
        }

        // Spawn the touched particle effect where the pickup is
        // Only if one exists
        if (m_touchedParticle != PooledDB.Particle.None)
        {
            PooledDB.Instance.Spawn(m_touchedParticle, transform.position, true);
        }

        // Self store this pickup if it applies
        SelfStore();        
    }

    // Pick a random color from the ColorSet and assign it
    void RandomizeColor()
    {
        int newColor = Random.Range(0, m_colorSet.Length - 1);

        SetColor(newColor);
    }

    // Change the color to the next color in the ColorSet
    // Note this counter is global for all konpeitos in the scene
    void CycleColor()
    {
        int newColor = (m_currentColor++) % m_colorSet.Length;
        SetColor(newColor);
    }

    /// <summary>
    /// Changes the colour of the konpeito to one in the ColorSet
    /// </summary>
    /// <param name="newColor">Must be a valid index in the ColorSet</param>
    void SetColor(int newColor)
    {
        if (m_colorSet[newColor] == null)
        {
            Debug.LogWarning("Konpeito ColorSet not complete");
            return;
        }

        m_meshRend.material = m_colorSet[newColor];

        m_light.color = m_colorSet[newColor].color;   // Use this with emissive shader
        //m_light.color = ColorSet[newColor].GetColor("_ReflectionTint");   // Use this is using reflective shader

        m_trailRender.material.SetColor(m_trailShaderColorID, m_colorSet[newColor].color);
    }
}