// Collectible class
// 

using UnityEngine;
using UnityEngine.Assertions;

public class Konpeito : BasePickUp
{
    #region Inspector
    [Header("Konpeito settings")]
    // Whether this drop is added individually or as a mass spawned event
    // Affects how the audio is played
    [SerializeField]
    bool m_massSpawned = false;
    // The available colour materials to cycle through on spawn
    [SerializeField]
    Material[] m_colorSet;

    [Header("Konpeito gameplay")]
    // Possible values for the konpeito to be worth
    [SerializeField]
    int[] m_values = { 1, 2, 5, 10, 15, 20 };
    // The default pitch shift
    [SerializeField]
    [Range(-12, 12)]
    int m_defaultPitch = 0;
    // The limit of the pitch shift
    [SerializeField]
    [Range(-12, 12)]
    int m_endPitch = 2;
    // How long before the pitch shift resets to normal
    [SerializeField]
    float m_pitchResetDelay = 5.0f;
    [SerializeField]
    float m_pitchStep = 0.5f;
    #endregion
    
    // Used to cycle through the available colours
    static int m_currentColor = 0;
    // cache propety id of konpeito's material's shader's color
    int m_trailShaderColorID = -1;
    // Used to cycle through the pick up sounds
    static float m_pitchDiff = 0;
    static float m_pitchStartTime = 0.0f;

    protected override void Awake()
    {
        base.Awake();
        
        m_trailRender = GetComponent<TrailRenderer>();
        
        Assert.IsNotNull(m_trailRender);

        m_trailShaderColorID = Shader.PropertyToID("_TintColor");         
        CycleColor();

        m_pitchDiff = m_defaultPitch;
    }    

    void Update()
    {
        if (Time.time - m_pitchResetDelay > m_pitchStartTime)
        {
            m_pitchDiff = m_defaultPitch;
        }
    }

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

        base.PickUp();
    }

    protected override void PlaySFX()
    {
        if (m_massSpawned)
        {
            AudioPool.Instance.PlayRandom(m_touchedSFX, transform.position, m_SFXSettings);
        }
        else
        {
            AudioSource test = AudioPool.Instance.PlayPitchShift(m_touchedSFX, transform.position, m_SFXSettings
                , m_pitchDiff, "CollectiblesPitchShift");

            // Only shift the pitch if the audio was successfully played
            if (test != null)
            {
                m_pitchDiff = (m_pitchDiff + m_pitchStep);
                m_pitchDiff = Mathf.Clamp(m_pitchDiff, m_defaultPitch, m_endPitch);
                m_pitchStartTime = Time.time;
            }
        }
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