﻿// Attach this to a prefab/gameobject containing a particle system
// This object will store itself back into the objectpool after a period of time
// Note: The particlesystem should be set to "Play on awake" and "looping off" for best ease of use
// If looping is enabled it will never store itself until you do it manually somehow or the level changes

using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using MovementEffects;


public class SelfStoreParticleEffect : CustomBehaviour
{
    void Awake()
    {
        Assert.IsNotNull(GetParticlSys);    
    }

    protected override void Start()
    {
        Assert.IsNotNull(m_parentPool);

        base.Start();
    }

    void OnEnable()
    {
        Timing.RunCoroutine(_CheckIfAlive(), Segment.SlowUpdate);        
    }

    IEnumerator<float> _CheckIfAlive()
    {
        while (true)
        {
            // When the effect is finished, store it back in the pool
            if (!GetParticlSys.IsAlive(true))
            {
                if (m_parentPool != null)
                {
                    SelfStore();
                }
                else
                {
                    Debug.LogWarning("SelStoreParticle had no pool to store: " + this.gameObject);
                    Debug.LogWarning("Destroying: " + this.gameObject);
                    Destroy(this.gameObject);                    
                }

                yield break;
            }

            yield return 0f;
        }
    }
}
