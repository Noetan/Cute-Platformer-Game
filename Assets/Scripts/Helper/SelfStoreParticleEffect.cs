// Attach this to a prefab/gameobject containing a particle system
// This object will store itself back into the objectpool after a period of time

using UnityEngine;
using System.Collections;
using MemoryManagment;

public class SelfStoreParticleEffect : CustomBehaviour
{
    ParticleSystem m_particleSystem;

    void Start()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        StartCoroutine("CheckIfAlive");
    }

    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            // Check every half second if the particle effect is still running
            yield return new WaitForSeconds(0.5f);

            // When the effect is finished, store it back in the pool
            if (!m_particleSystem.IsAlive(true))
            {
                if (m_parentPool != null)
                {
                    m_parentPool.Store(this.gameObject);
                }
                else
                {
                    Debug.LogWarning("SelStoreParticle had no pool to store: " + this.gameObject);
                }
            }
        }
    }    
}
