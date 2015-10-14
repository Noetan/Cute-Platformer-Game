// Adds additional features to the default MonoBehaviour class
// For 2D sprites only

using UnityEngine;
using System.Collections;
using MemoryManagment;

public class CustomBehaviour : MonoBehaviour
{
    protected GameObjectPool m_parentPool;

    public CustomBehaviour()
    {
    }

    public void SetPool(GameObjectPool newPool)
    {
        m_parentPool = newPool;
    }

    public virtual void Reset()
    {

    }
}