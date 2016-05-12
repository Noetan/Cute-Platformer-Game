// Attach this to a prefab/gameobject containing a particle system
// This object will store itself back into the objectpool after a period of time
// Note: The particlesystem should be set to "Play on awake" and "looping off" for best ease of use
// If looping is enabled it will never store itself until you do it manually somehow or the level changes

using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using MovementEffects;


public class SelfStoreParticleEffect : CustomBehaviour
{
    protected override void Awake()
    {
        m_particleSystem = GetComponent<ParticleSystem>();

        Assert.IsNotNull(m_particleSystem);

        base.Awake();
    }

    protected override void Start()
    {
        Assert.IsNotNull(m_parentPool);

        base.Start();
    }

    protected override void OnEnable()
    {
        Timing.RunCoroutine(_CheckIfAlive(), Segment.SlowUpdate);        
    }

    IEnumerator<float> _CheckIfAlive()
    {
        while (true)
        {
            // When the effect is finished, store it back in the pool
            if (!m_particleSystem.IsAlive(true))
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
