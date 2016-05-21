// Adds additional features to the default MonoBehaviour class
// For 2D sprites only

using UnityEngine;
using System.Collections.Generic;
using MemoryManagment;
using MovementEffects;

public class CustomBehaviour : MonoBehaviour
{
    protected GameObjectPool m_parentPool;

    protected MeshRenderer m_meshRend = null;
    protected TrailRenderer m_trailRender = null;
    protected Light m_light = null;
    protected ParticleSystem m_particleSystem = null;
    protected AudioSource m_AudioSource = null;

    protected bool m_startDone = false;

    #region Unity Functions
    protected virtual void Awake()
    {
        m_meshRend = GetComponent<MeshRenderer>();
        m_trailRender = GetComponent<TrailRenderer>();
        m_light = GetComponent<Light>();
        m_particleSystem = GetComponent<ParticleSystem>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    protected virtual void Start()
    {
        m_startDone = true;
    }
    #endregion

    public void SetPool(GameObjectPool newPool)
    {
        m_parentPool = newPool;
    }

    public virtual void Reset()
    {

    }

    // Use this to store this gameobject back into its pool
    public void SelfStore(float delay = 0f)
    {
        if (m_parentPool == null)
        {
            Debug.LogWarning("CustomBehaviour tried to selfstore but doesnt have a parent. Destroying..." + gameObject.GetInstanceID());
            Destroy(this.gameObject);
            return;
        }

        Timing.RunCoroutine(_WaitAndStore(delay));
    }

    public AudioSource GetAudioSource
    {
        get
        {
            return m_AudioSource;
        }
    }

    #region Internals
    IEnumerator<float> _WaitAndStore(float delay)
    {
        if (delay > 0)
        {
            yield return Timing.WaitForSeconds(delay);
        }

        m_parentPool.Store(this.gameObject);
    }
    #endregion
}