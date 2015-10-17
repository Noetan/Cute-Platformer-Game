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

public class AttractedItem : MonoBehaviour
{
    #region Inspector Variables
    // How strongly the item is attracted
    [SerializeField]
    float m_attractStrength = 400f;
    // The item being attracted
    [SerializeField]
    GameObject m_item;
    #endregion

    enum State
    {
        idle,
        active,
        finished
    }
    State m_currentState = State.idle;

    // The player we're attracting towards
    GameObject m_player = null;
    // The item's rigidbody
    Rigidbody m_rigidbody = null;

    void Start()
    {
        Assert.IsNotNull(m_item);
        
        m_rigidbody = GetComponentInParent<Rigidbody>();
    }
	
	void FixedUpdate ()
    {
	    if (m_currentState == State.active)
        {
            Assert.IsNotNull(m_rigidbody);
            Assert.IsNotNull(m_player);

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
        }
	}

    // Check for when the player gets close enough
    void OnTriggerEnter(Collider other)
    {
        if (m_currentState == State.idle)
        {
            if (other.CompareTag("Player"))
            {
                m_currentState = State.active;
                m_player = other.gameObject;
                Debug.Log("attracting");
            }
        }
    }

    public void Reset()
    {
        m_currentState = State.idle;
        gameObject.SetActive(true);
        m_rigidbody.isKinematic = false;
    }
}
