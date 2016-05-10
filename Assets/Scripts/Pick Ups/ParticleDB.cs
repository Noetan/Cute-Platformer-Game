// A centralized database of SelfStoreParticles
// To add a new particle
// 1. Add an entry into the Types enum
// 2. Keep a reference to the SelfStore prefab
// 3. Create a new objectpool for it and init it
//
// To spawn a particle effect
// Use the public Spawn function. e.g.
// Gameobject newGObj = ParticleDB.Instance.Spawn(ParticleDB.Type.JumpAirPuff);
// newGObj.transform.position = this.transform.position;
// newGObj.SetActive(true);
//
// The SelfStoreParticle will then store itself back for reuse when it's done playing

using UnityEngine;
using MemoryManagment;

public class ParticleDB : MonoBehaviour 
{
    public static ParticleDB Instance { get; set; }

    // 1.
    public enum Type
    {
        None,
        DropsPickUpShatter,
        JumpGroundPuff,
        JumpAirPuff
    }

    // 2.
    [SerializeField]
    GameObject m_DropsPickUpShatter;


    // 3.
    GameObjectPool dropsPickUpShatterPool;

    void Awake()
    {
        Instance = this;

        // Init pools here
        dropsPickUpShatterPool = new GameObjectPool(0, m_DropsPickUpShatter, this.gameObject);
    }

    public GameObject Spawn(Type type)
    {
        switch (type)
        {
            case Type.DropsPickUpShatter:
                return dropsPickUpShatterPool.New();

            default:
                Debug.LogWarning("ParticleDB Spawn() requested an invalid Type");
                return null;
        }
    }
}
