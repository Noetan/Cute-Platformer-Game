// Adds additional features to the default MonoBehaviour class
// For 2D sprites only

using UnityEngine;
using System.Collections;
using MemoryManagment;

public class CustomBehaviour : MonoBehaviour
{
    protected GameObjectPool m_parentPool;

    protected MeshRenderer m_meshRend = null;
    protected TrailRenderer m_trailRender = null;
    protected Light m_light = null;

    #region Unity Functions
    public CustomBehaviour()
    {
    }

    protected virtual void Start()
    {        
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
    #endregion

    public void SetPool(GameObjectPool newPool)
    {
        m_parentPool = newPool;
    }

    public virtual void Reset()
    {

    }
}