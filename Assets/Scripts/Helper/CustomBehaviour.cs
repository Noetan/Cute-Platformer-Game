// Adds additional features to the default MonoBehaviour class
// For 2D sprites only

using UnityEngine;
using System.Collections.Generic;
using MemoryManagment;
using MovementEffects;

public class CustomBehaviour : MonoBehaviour
{
    protected GameObjectPool m_parentPool;

    MeshRenderer m_meshRend = null;
    TrailRenderer m_trailRender = null;
    Light m_light = null;
    ParticleSystem m_particleSystem = null;
    AudioSource m_AudioSource = null;

    protected bool m_startDone = false;

    #region Unity Functions
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
            if (m_AudioSource == null)
            {
                m_AudioSource = GetComponent<AudioSource>();
            }

            return m_AudioSource;
        }
    }

    protected ParticleSystem GetParticlSys
    {
        get
        {
            if (m_particleSystem == null)
            {
                m_particleSystem = GetComponent<ParticleSystem>();
            }

            return m_particleSystem;
        }
    }

    protected Light GetLight
    {
        get
        {
            if (m_light == null)
            {
                m_light = GetComponent<Light>();
            }

            return m_light;
        }
    }

    protected TrailRenderer GetTrailRenderer
    {
        get
        {
            if (m_trailRender == null)
            {
                m_trailRender = GetComponent<TrailRenderer>();
            }

            return m_trailRender;
        }
    }

    protected MeshRenderer GetMeshRenderer
    {
        get
        {
            if (m_meshRend == null)
            {
                m_meshRend = GetComponentInChildren<MeshRenderer>();
            }

            return m_meshRend;
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