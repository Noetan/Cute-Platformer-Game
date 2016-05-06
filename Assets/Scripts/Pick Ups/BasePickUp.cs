// Base class for world items that can be "picked up" by touching them with the player
using UnityEngine;

public class BasePickUp : CustomBehaviour
{
    #region Inspector Variables
    // The sound effect that plays when the item is picked up
    [SerializeField]
    AudioSource m_soundEffect;
    // The particle effect that plays when the item is picked up
    [SerializeField]
    ParticleSystem m_touchedParticleEffect;

    // Does the item respawn even after it's been picked up before?
    [SerializeField]
    bool m_respawns = true;

    [Header("Animation")]
    // Does the item rotate when active
    [SerializeField]
    bool m_rotate = false;
    [SerializeField]
    float m_rotateSpeedMin = 50f;
    [SerializeField]
    float m_rotateSpeedMax = 100f;

    // Does the item float/bob up and down when active
    [SerializeField]
    bool m_bob = false;
    [SerializeField]
    float m_bobSpeedMin = 1.0f;
    [SerializeField]
    float m_bobSpeedMax = 1.5f;
    [SerializeField]
    float m_bobHeight = 1.0f;
    #endregion

    enum State
    {
        Idle,
        Active
    }
    State m_currentState = State.Idle;

    float m_rotateSpeed = 0.0f;
    float m_bobSpeed = 0.0f;
    Vector3 m_defaultPosition = Vector3.zero;

    protected override void Start () 
	{
        Reset();

        m_defaultPosition = transform.position;
        m_rotateSpeed = Random.Range(m_rotateSpeedMin, m_rotateSpeedMax);
        m_bobSpeed = Random.Range(m_bobSpeedMin, m_bobSpeedMax);

        base.Start();  
	} 

    protected virtual void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag(PlayerController.Tag) )
        {
            PickUp();
        }
    }

    protected virtual void PickUp()
    {
        // Disable the item
        gameObject.SetActive(false);

        // play sound effect
        if (m_soundEffect != null)
        {
            m_soundEffect.Play();
        }

        // play particle effect
        if (m_touchedParticleEffect != null)
        {
            m_touchedParticleEffect.Play();
        }
    }

    public override void Reset()
    {
        // If the item is only meant to be picked up once per save file
        if (!m_respawns)
        {
            // Check if the player has already picked it up
            //if (false) // TO BE IMPLEMENTED
            {
                // If they have don't spawn the item
                //gameObject.SetActive(false);

                //break;
            }
        }

        m_currentState = State.Idle;
        gameObject.SetActive(true);
    }

    protected override void Update()
    {
        switch(m_currentState)
        {
            case State.Idle:
                if (m_rotate)
                {
                    transform.Rotate(Vector3.up, m_rotateSpeed * Time.deltaTime);
                }
                if (m_bob)
                {
                    float newPosY = (Mathf.Sin(Time.time * m_bobSpeed) * m_bobHeight);
                    transform.position = m_defaultPosition + new Vector3(0.0f, newPosY, 0.0f);
                }
                break;
        }

        base.Update();
    }

    // Allows item to follow player if being attracted
    public void Activate()
    {
        m_currentState = State.Active;
    }
}
