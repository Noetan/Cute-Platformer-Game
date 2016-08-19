// Base class for world items that can be "picked up" by touching them with the player
using UnityEngine;
using UnityEngine.Assertions;

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
    Helper.RangeFloat m_rotateSpeedRange;

    // Does the item float/bob up and down when active
    [SerializeField]
    bool m_bob = false;
    [SerializeField]
    Helper.RangeFloat m_bobSpeedRange;
    [SerializeField]
    float m_bobHeight = 1.0f;

    [Header("Effects")]
    [SerializeField]
    protected Pools.Particles m_touchedParticle = Pools.Particles.None;
    [SerializeField]
    protected AudioClip[] m_touchedSFX;
    [SerializeField]
    protected AudioClipSettings m_SFXSettings;
    #endregion

    public enum State
    {
        Idle,
        Active,
        Disabled
    }
    public State CurrentState { get; protected set; }

    float m_rotateSpeed = 0.0f;
    float m_bobSpeed = 0.0f;
    Vector3 m_defaultPosition = Vector3.zero;

    protected virtual void Awake()
    {
        m_rotateSpeed = Random.Range(m_rotateSpeedRange.Min, m_rotateSpeedRange.Max);
        m_bobSpeed = Random.Range(m_bobSpeedRange.Min, m_bobSpeedRange.Max);

        CurrentState = State.Idle;

        Assert.IsNotNull(GetMeshRenderer);
        Assert.IsNotNull(GetLight);
    }

    protected override void Start()
    {
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].isTrigger)
            {
                Physics.IgnoreCollision(colliders[i], PlayerController.MainCollider, true);
            }
        }

        base.Start();
    }

    void OnEnable () 
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
        //ShowModel(true);

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
        //ShowModel(false);

        PlaySFX();

        PlayParticleFX();

        // Self store this pickup if it applies
        SelfStore();
    }

    void FixedUpdate()
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
    }

    protected virtual void PlaySFX()
    {
        if (m_touchedSFX.Length > 0)
        {
            AudioPool.Instance.PlayRandom(m_touchedSFX, transform.position, m_SFXSettings);
        }
    }

    protected virtual void PlayParticleFX()
    {
        // Spawn the touched particle effect where the pickup is
        // Only if one exists
        if (m_touchedParticle != Pools.Particles.None)
        {
            PooledDB.Instance.Spawn(m_touchedParticle, transform.position, true);
        }
    }


    // Allows item to follow player if being attracted
    public void Activate()
    {
        CurrentState = State.Active;
    }

    // Unused for now, not needed?
    void ShowModel(bool enable)
    {
        GetMeshRenderer.enabled = enable;

        if (GetLight != null)
        {
            GetLight.enabled = enable;
        }

        if (GetParticlSys != null)
        {
            if (enable)
            {
                GetParticlSys.Play();
            }
            else
            {
                GetParticlSys.Stop();
            }
        }
    }
}
