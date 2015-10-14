// Base class for world items that can be "picked up" by touching them with the player
using UnityEngine;

public class BasePickUp : CustomBehaviour
{
    #region Inspector Variables
    // The sound effect that plays when the item is picked up
    [SerializeField]
    AudioClip m_soundEffect;
    // The particle effect that plays when the item is picked up
    [SerializeField]
    ParticleSystem m_touchedParticleEffect;

    // Does the item respawn even after it's been picked up before?
    [SerializeField]
    bool m_respawns = true;
    #endregion

    enum State
    {
        Idle,
        PrepareInactive,
        Inactive
    }
    State m_currentState = State.Idle;
	
	void Start () 
	{
        /*
        // If the item is only meant to be picked up once per save file
        if (!m_respawns)
        {
            // Check if the player has already picked it up
            if (false) // TO BE IMPLEMENTED
            {
                // If they have don't spawn the item
                gameObject.SetActive(false);
            }
        }
        */	
	}
	
	void Update () 
	{
        switch (m_currentState)
        {
            case State.Idle:
                break;
            case State.PrepareInactive:
                if (m_touchedParticleEffect.isStopped)
                {
                    m_currentState = State.Inactive;
                    gameObject.SetActive(false);
                }
                break;
            case State.Inactive:
                break;
        }
	
	}

    protected virtual void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag("Player") )
        {
            PickUp();
        }
    }


    public virtual void PickUp()
    {
        // play sound effect

        // play particle effect
        if (m_touchedParticleEffect)
        {
            m_touchedParticleEffect.Play();
            m_currentState = State.PrepareInactive;
        }
        else
        {
            m_currentState = State.Inactive;
        }

        MeshRenderer meshRen = GetComponent<MeshRenderer>();
        meshRen.enabled = false;
    }

    public override void Reset()
    {

    }
}
