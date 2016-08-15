// A centralized database of SelfStoreParticles and pick up items
// To add a new entry
// 1. Add an entry into the Types enum
// 2. Assign it a prefab in the inspector
//
// To spawn something
// Use the public Spawn function. e.g.
// Gameobject newGObj = PooledDB.Instance.Spawn(PooledDB.Particle.PlayerJumpAir, this.transform.position, true);
//
// If particle, the SelfStoreParticle will then store itself back for reuse when it's done playing so no worries

using UnityEngine;
using UnityEngine.Assertions;
using MemoryManagment;
using System;

public class PooledDB : MonoBehaviour 
{
    /*
    // Add your entries here. Don't forget to assign the prefab in the inspector
    public enum Particle
    {
        None,
        DropsTouchShatter,
        PlayerJumpGround,
        PlayerJumpAir,
        DropsSpawnShine
    }
    public enum PickUp
    {
        None,
        Drop,
        Health,
        DropsSpawner,
        DropMass
    }*/

    // NO NEED TO TOUCH ANYTHING DOWN HERE ANYMORE
    #region Internal variables
    // Reference to the prefabs used to spawn things
    [SerializeField]
    GameObject[] m_ParticlePrefabs = new GameObject[Enum.GetValues(typeof(Pools.Particles)).Length];
    [SerializeField]
    GameObject[] m_PickUpsPrefabs = new GameObject[Enum.GetValues(typeof(Pools.PickUps)).Length];
    
    // Used to access PooledDB from anywhere in the code
    public static PooledDB Instance { get; set; }

    GameObjectPool[] m_Particles = new GameObjectPool[Enum.GetValues(typeof(Pools.Particles)).Length];
    GameObjectPool[] m_PickUps = new GameObjectPool[Enum.GetValues(typeof(Pools.PickUps)).Length];


    void Awake()
    {
        Assert.IsNull(Instance, "More than 1 instance of PooledDB detected. ONLY HAVE 1 IN THE SCENE PLZ");
        Instance = this;

        Assert.AreEqual(m_ParticlePrefabs.Length, Helper.CountEnum(typeof(Pools.Particles)));
        Assert.AreEqual(m_PickUpsPrefabs.Length, Helper.CountEnum(typeof(Pools.PickUps)));

        // Set up the object pools for each entry
        for (int i = 0; i < Helper.CountEnum(typeof(Pools.Particles)); i++)
        {
            m_Particles[i] = new GameObjectPool(0, m_ParticlePrefabs[i], this.gameObject);
        }
        for (int i = 0; i < Helper.CountEnum(typeof(Pools.PickUps)); i++)
        {
            m_PickUps[i] = new GameObjectPool(0, m_PickUpsPrefabs[i], this.gameObject);
        }
    }
    #endregion

    // Spawn methods    
    public GameObject Spawn(Pools.Particles type, Vector3 pos, bool active)
    {
        var GO = m_Particles[(int)type].New(pos);
        GO.SetActive(active);
        return GO;
    }
    public GameObject Spawn(Pools.PickUps type, Vector3 pos, bool active)
    {
        var GO = m_PickUps[(int)type].New(pos);
        GO.SetActive(active);
        return GO;
    }
}
