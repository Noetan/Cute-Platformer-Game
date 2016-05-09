// Used to keep track and store all the collectibles in the game

using UnityEngine;
using MemoryManagment;

public class PickUpController : MonoBehaviour 
{
    public static PickUpController Instance;

    #region Inspector
    [SerializeField]
    GameObject m_DropsPrefab;
    #endregion

    GameObjectPool m_dropsPool;

    public enum Types
    {
        drops,
        health
    }

    void Awake()
    {
        Instance = this;
        m_dropsPool = new GameObjectPool(1, m_DropsPrefab, this.gameObject);
    }

    public GameObject Spawn(Types newPup)
    {
        switch (newPup)
        {
            case Types.drops:
                return m_dropsPool.New();

            case Types.health:
                Debug.LogWarning("Requested health pickup from controller. Not implemented yet");
                return null;

            default:
                Debug.LogWarning("Requested invalid pick up type from pickupcontroller");
                return null;
        }
    }
}
