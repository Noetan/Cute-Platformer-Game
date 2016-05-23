// Cycles the starting color of a particle system through all the colors over time
// Only changes the hue so the "brightness" of the color doesnt change

using UnityEngine;
using UnityEngine.Assertions;

public class CycleParticleStartColor : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_cycleSpeed = 0.01f;

    const int HUE_CAP = 1;

    ParticleSystem m_ParticleSys;
    Vector3 m_startHSV = Vector3.zero;

    float m_hueDiff = 0.0f;

    void Awake()
    {
        m_ParticleSys = GetComponent<ParticleSystem>();
        Assert.IsNotNull(m_ParticleSys);
    }

    void Start()
    {
        Color.RGBToHSV(m_ParticleSys.startColor, out m_startHSV.x, out m_startHSV.y, out m_startHSV.z);
    }
    
	void FixedUpdate ()
    {
        m_hueDiff = (m_hueDiff + (m_cycleSpeed)) % HUE_CAP;
        m_ParticleSys.startColor = Color.HSVToRGB(m_startHSV.x + m_hueDiff, m_startHSV.y, m_startHSV.z);
    }
}
