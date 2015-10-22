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
    #endregion

    bool m_defaultGravity = false;

    enum State
    {
        Idle,
        Inactive
    }
    State m_currentState = State.Idle;
	
    void Awake()
    {
    }

	void Start () 
	{
        Reset();      
	}
	
	void Update () 
	{
        switch (m_currentState)
        {
            case State.Idle:
                break;
            case State.Inactive:
                break;
        }
	
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

        m_currentState = State.Inactive;
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
}
