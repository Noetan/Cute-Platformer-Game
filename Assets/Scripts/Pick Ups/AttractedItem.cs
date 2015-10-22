// Use alongside BasePickup or inheritor of BasePickup
// Though technically this can be used with any gameobject
// Bear in the mind the attraction will only stop once the other gameobject has been disabled
// Be careful that they're not parented to each other

// Here's how it should be set out
// Parent (rigidbody) -> Child 1 & Child 2
// Child 1 (model, idle animation and effects, BasePickUp.cs, trigger collider for picking up the item)
// Child 2 (the touched effects like AudioSource, ParticleSystem, AttractedItem.cs and the trigger collider to set off the attraction)

// Make sure the drag on the parent's rigidbody is high enough (something like 20) to avoid items orbitting forever

using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class AttractedItem : MonoBehaviour
{    
    #region Inspector Variables
    [Header("Required")]
    // The item being attracted
    [SerializeField]
    GameObject m_item;

    [Header("Properties")]
    // How strongly the item is attracted
    [SerializeField]
    float m_attractStrength = 450f;
    [SerializeField]
    float m_initalJumpHeight = 25f;
    #endregion

    enum State
    {
        idle,
        starting,
        active,
        finished
    }
    State m_currentState = State.idle;

    // The player we're attracting towards
    GameObject m_player = null;
    // The item's rigidbody
    Rigidbody m_rigidbody = null;
    bool m_defaultGravity = false;

    void Start()
    {
        Assert.IsNotNull(m_item);

        m_rigidbody = GetComponentInParent<Rigidbody>();
        m_defaultGravity = m_rigidbody.useGravity;
        
        Assert.IsNotNull(m_rigidbody);
    }

    void FixedUpdate()
    {
        switch (m_currentState)
        {
            case State.starting:
                // Make the item hop up before starting to follow the player
                m_currentState = State.active;
                m_rigidbody.useGravity = false;              

                m_rigidbody.AddForce(0, m_initalJumpHeight, 0, ForceMode.VelocityChange);
                
                break;

            case State.active:
                // Stop attracting once the item has been picked up
                if (!m_item.activeInHierarchy)
                {
                    m_currentState = State.finished;
                    m_rigidbody.isKinematic = true;
                    m_rigidbody.velocity = Vector3.zero;
                    return;
                }

                // Get the direction to the player
                // Multiply the direction with the strength to get the force
                // Apply the force
                Vector3 newDir = m_player.transform.position - gameObject.transform.position;
                newDir = newDir.normalized * m_attractStrength;
                m_rigidbody.AddForce(newDir);
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        OnTriggerStay(other);
    }

    // Check for when the player gets close enough
    void OnTriggerStay(Collider other)
    {
        if (m_currentState == State.idle && other.CompareTag("Player"))
        {
            m_player = other.gameObject;            

            // Check if there is a wall between the player and the item
            RaycastHit hitInfo;
            if (Physics.Linecast(gameObject.transform.position, m_player.transform.position, out hitInfo, LayerMask.NameToLayer("") , QueryTriggerInteraction.Ignore))
            { 
                Debug.DrawLine(gameObject.transform.position, m_player.transform.position, Color.red, 60);
                //Debug.Log(string.Format("blocked {0}, {1}", gameObject.transform.position, m_player.transform.position), gameObject);     
                //Debug.Log(hitInfo.collider + " " + hitInfo.collider.tag);
                //Debug.Log(LayerMask.NameToLayer(""));


                // If there is, dont start attracting
                if (!hitInfo.collider.CompareTag("Player"))
                {
                    return;
                }
            }

            // If not, start attracting to the player
            m_currentState = State.starting;

            Assert.IsNotNull(m_player);
        }
    }

    public void Reset()
    {
        m_currentState = State.idle;
        gameObject.SetActive(true);
        m_rigidbody.isKinematic = false;
        m_rigidbody.useGravity = m_defaultGravity;
        m_rigidbody.detectCollisions = true;
    }
}
