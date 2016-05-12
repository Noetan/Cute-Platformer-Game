// Used to keep track and store all the collectibles in the game

// To add new pick up items
// 1. Add to the Types enum
// 2. Assign a prefab
// 3. Create a pool for it
// 4. Add an entry in the Spawn()

using UnityEngine;
using MemoryManagment;

public class PickUpDB : MonoBehaviour 
{
    public static PickUpDB Instance;

    // 1.
    public enum Type
    {
        none,
        drops,
        health,
        dropsSpawner
    }

    // 2.
    #region Inspector
    [SerializeField]
    GameObject m_DropsPrefab;
    [SerializeField]
    GameObject m_dropsSpawner;
    [SerializeField]
    GameObject m_none;
    #endregion

    // 3.
    GameObjectPool m_dropsPool;
    GameObjectPool m_dropsSpawnerPool;
    GameObjectPool m_nonePool;
    
    void Awake()
    {
        Instance = this;

        // 3.
        m_dropsPool = new GameObjectPool(0, m_DropsPrefab, this.gameObject);
        m_dropsSpawnerPool = new GameObjectPool(0, m_dropsSpawner, this.gameObject);
        m_nonePool = new GameObjectPool(0, m_none, this.gameObject);
    }

    // 4.
    public GameObject Spawn(Type newPup)
    {
        switch (newPup)
        {
            case Type.drops:
                return m_dropsPool.New();

            case Type.health:
                Debug.LogWarning("Requested health pickup from controller. Not implemented yet");
                return null;

            case Type.dropsSpawner:
                return m_dropsSpawnerPool.New();

            case Type.none:
                return m_nonePool.New();

            default:
                Debug.LogWarning("Requested invalid pick up type from pickupcontroller");
                return null;
        }
    }
}
