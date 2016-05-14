// Base class for world items that can be "picked up" by touching them with the player
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using MovementEffects;

public class BasePickUp : CustomBehaviour
{
    #region Inspector Variables
    [Header("Gameplay")]
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

    [SerializeField]
    PooledDB.Particle m_touchedParticle = PooledDB.Particle.None;
    #endregion

    public enum State
    {
        Idle,
        Active,
        Disabled
    }
    public State CurrentState { get; private set; }

    float m_rotateSpeed = 0.0f;
    float m_bobSpeed = 0.0f;
    Vector3 m_defaultPosition = Vector3.zero;

    protected override void Awake()
    {
        m_meshRend = GetComponent<MeshRenderer>();
        m_particleSystem = GetComponent<ParticleSystem>();
        m_light = GetComponent<Light>();

        Assert.IsNotNull(m_meshRend);
        Assert.IsNotNull(m_light);

        m_rotateSpeed = Random.Range(m_rotateSpeedMin, m_rotateSpeedMax);
        m_bobSpeed = Random.Range(m_bobSpeedMin, m_bobSpeedMax);

        CurrentState = State.Idle;

        base.Awake();        
    }

    protected override void OnEnable () 
	{
        /// Note this gets called multiple times at the start of the level for some reason ///
        /// Keep that in mind when doing things here, nothing that must only be called once ///

        // If the item is only meant to be picked up once per save file
        if (!m_respawns)
        {
            // Check if the player has already picked it up
            //if (false) // TO BE IMPLEMENTED
            {
                // If they have don't spawn the item
                //break;
            }
        }

        m_defaultPosition = transform.position;
        CurrentState = State.Idle;
        ShowModel(true);
        //Debug.Log("basepickup start");

        base.Start();  
	} 

    protected virtual void OnTriggerEnter(Collider other)
    {
        if ( other.CompareTag(PlayerController.Tag) && CurrentState != State.Disabled)
        {
            PickUp();
        }
    }

    protected virtual void PickUp()
    {
        // Disable the item
        CurrentState = State.Disabled;
        ShowModel(false);

        // play sound effect

        // Spawn the touched particle effect where the pickup is
        // Only if one exists
        if (m_touchedParticle != PooledDB.Particle.None)
        {
            PooledDB.Instance.Spawn(m_touchedParticle, transform.position, true);
        }

        // Self store this pickup if it applies
        SelfStore();
    }

    protected override void FixedUpdate()
    {
        switch(CurrentState)
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
        CurrentState = State.Active;
    }
}
