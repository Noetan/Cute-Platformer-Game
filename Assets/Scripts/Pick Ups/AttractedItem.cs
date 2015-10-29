// Use alongside BasePickup or inheritor of BasePickup
// Though technically this can be used with any gameobject
// Bear in the mind the attraction will only stop once the other gameobject has been disabled
// Be careful that they're not parented to each other

// Here's how it should be set out
// Parent (rigidbody) -> Child 1 & Child 2
// Child 1 (model, idle animation and effects, BasePickUp.cs, trigger collider for picking up the item)
// Child 2 (the touched effects like AudioSource, ParticleSystem, AttractedItem.cs and the trigger collider to set off the attraction)

// Assign the object (e.g. the player) you want to attract to m_item.
// Assign the pick up's model collider to m_collider

// Make sure the drag on the parent's rigidbody is high enough (something like 20) to avoid items orbitting forever

using UnityEngine;
using UnityEngine.Assertions;
using System.Collections; // Enumerators

[RequireComponent(typeof(Rigidbody))]
public class AttractedItem : MonoBehaviour
{    
    #region Inspector Variables
    [Header("Required")]
    // The item being attracted
    [SerializeField]
    GameObject m_item;
    // The item's collider
    [SerializeField]
    Collider m_itemCollider;

    [Header("Properties")]
    // How strongly the item is attracted
    [SerializeField]
    float m_attractStrength = 450f;
    // How strong the item jumps up before it starts attracting
    [SerializeField]
    float m_initalJumpHeight = 55f;
    // How long after it first jumps before it starts attracting
    [SerializeField]
    float m_jumpDelay = 0.2f;
    #endregion

    enum State
    {
        idle,
        starting,
        active,
        finished
    }
    State m_currentState = State.idle;

    // The object we're attracting towards
    GameObject m_goal = null;
    // The item's rigidbody
    Rigidbody m_rigidbody = null;
    bool m_defaultGravity = false;

    void Start()
    {
        Assert.IsNotNull(m_item);

        m_rigidbody = GetComponentInParent<Rigidbody>();
        m_goal = PlayerController.Player;        
        
        Assert.IsNotNull(m_rigidbody);
        Assert.IsNotNull(m_goal);

        m_defaultGravity = m_rigidbody.useGravity;
    }

    void FixedUpdate()
    {
        switch (m_currentState)
        {
            case State.starting:
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
                Vector3 newDir = m_goal.transform.position - gameObject.transform.position;
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
        if (m_currentState == State.idle && other.CompareTag(m_goal.tag))
        {
            // Check if there is a wall between the player and the item
            RaycastHit hitInfo;
            if (Physics.Linecast(gameObject.transform.position, m_goal.transform.position
                , out hitInfo, LayerMask.NameToLayer("") , QueryTriggerInteraction.Ignore))
            { 
                //Debug.DrawLine(gameObject.transform.position, m_goal.transform.position, Color.red, 60);
                //Debug.Log(string.Format("blocked {0}, {1}", gameObject.transform.position, m_player.transform.position), gameObject);     
                //Debug.Log(hitInfo.collider + " " + hitInfo.collider.tag);
                //Debug.Log(LayerMask.NameToLayer(""));

                // If there is, dont start attracting
                if (!hitInfo.collider.CompareTag(m_goal.tag))
                {
                    return;
                }
            }

            // If not, start attracting to the player
            // Make the item hop up before starting to follow the player
            StartCoroutine(Hop());
            m_currentState = State.starting;

            Assert.IsNotNull(m_goal);
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

    IEnumerator Hop()
    {
        m_rigidbody.AddForce(0, m_initalJumpHeight, 0, ForceMode.VelocityChange);

        yield return new WaitForSeconds(m_jumpDelay);

        InitActiveState();
    }

    void InitActiveState()
    {
        m_rigidbody.useGravity = false;
        m_currentState = State.active;
    }
}
