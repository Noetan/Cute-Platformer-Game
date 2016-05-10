﻿// Adds additional features to the default MonoBehaviour class
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

    protected bool m_startDone = false;

    #region Unity Functions
    public CustomBehaviour()
    {
    }

    protected virtual void Start()
    {
        m_startDone = true;
    }

    protected virtual void Awake()
    {
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void OnEnable()
    {
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
    protected void SelfStore(float delay = 0f)
    {
        if (m_parentPool == null)
        {
            Debug.LogWarning("CustomBehaviour tried to selfstore but doesnt have a parent. Destroying..." + gameObject.GetInstanceID());            
            Destroy(this.gameObject);
            return;
        }

        Timing.RunCoroutine(_WaitAndStore(delay));
    }

    IEnumerator<float> _WaitAndStore(float delay)
    {
        if (delay > 0)
        {
            yield return Timing.WaitForSeconds(delay);
        }

        m_parentPool.Store(this.gameObject);
    }

    // Use to enable and disable the looks of the gameobject
    // Affects the mesh (required) and any lighting and particle systems (if present)
    protected void ShowModel(bool enable)
    {
        m_meshRend.enabled = enable;

        if (m_light != null)
        {
            m_light.enabled = enable;
        }

        if (m_particleSystem != null)
        {
            if (enable)
            {
                m_particleSystem.Play();
            }
            else
            {
                m_particleSystem.Stop();
            }
        }
    }
}