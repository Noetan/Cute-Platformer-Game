// Used to keep track and store all the collectibles in the game

// To add new pick up items
// 1. Add to the Types enum
// 2. Assign a prefab
// 3. Create a pool for it

using UnityEngine;
using MemoryManagment;

public class PickUpDB : MonoBehaviour 
{
    public static PickUpDB Instance;

    #region Inspector
    [SerializeField]
    GameObject m_DropsPrefab;
    #endregion

    GameObjectPool m_dropsPool;

    public enum Type
    {
        drops,
        health
    }

    void Awake()
    {
        Instance = this;
        m_dropsPool = new GameObjectPool(0, m_DropsPrefab, this.gameObject);
    }

    public GameObject Spawn(Type newPup)
    {
        switch (newPup)
        {
            case Type.drops:
                return m_dropsPool.New();

            case Type.health:
                Debug.LogWarning("Requested health pickup from controller. Not implemented yet");
                return null;

            default:
                Debug.LogWarning("Requested invalid pick up type from pickupcontroller");
                return null;
        }
    }
}
