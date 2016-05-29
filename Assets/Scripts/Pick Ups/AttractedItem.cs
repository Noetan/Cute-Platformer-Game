// Use alongside BasePickup or inheritor of BasePickup
// Make sure the drag on the parent's rigidbody is high enough (something like 20) to avoid items orbitting forever
// While code can attract to something other than the player, there's no way to set that, add it if you need it
// It's m_goal
// Note, this supports pickups with a physical collider but the code has been commented out and might be out of date

using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using MovementEffects;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BasePickUp))]

public class AttractedItem : MonoBehaviour
{
    #region Inspector Variables
    [Header("Properties")]
    // How close can the player get before the attraction is triggered
    [SerializeField]
    float m_attractRadius = 3f;
    // How strongly the item is attracted
    [SerializeField]
    float m_attractStrength = 650f;
    // How strong the item jumps up before it starts attracting
    [SerializeField]
    float m_initalJumpHeight = 40f;
    // How long after it first jumps before it starts attracting
    [SerializeField]
    float m_jumpDelay = 0.25f;
    // How long after it explodes before the item start attracting
    [SerializeField]
    float m_explodeDelay = 0.75f;
    [SerializeField]
    AudioClip m_SFX;
    #endregion

    enum State
    {
        idle,
        starting,
        active,
        finished
    }
    State m_currentState = State.idle;

    public enum AttractMode
    {
        pickedup,
        explode
    }

    // The object we're attracting towards
    GameObject m_goal = null;
    // The item's rigidbody
    Rigidbody m_rigidbody = null;
    bool m_defaultGravity = false;
    // The item's colldiers
    //List<Collider> m_itemColliders = new List<Collider>();
    // The item's script    
    BasePickUp m_item = null;

    void Awake()
    {
        m_rigidbody = GetComponentInParent<Rigidbody>();
        m_item = GetComponent<BasePickUp>();
        Assert.IsNotNull(m_rigidbody);
        Assert.IsNotNull(m_item);

        m_defaultGravity = m_rigidbody.useGravity;

        /*
        // Get all the colliders of our pick up item
        Collider[] newColliders = m_item.GetComponents<Collider>();
        for (int i = 0; i < newColliders.Length; i++)
        {
            if (!newColliders[i].isTrigger)
            {
                m_itemColliders.Add(newColliders[i]);
            }
        }
        m_itemScript = m_item.GetComponent<BasePickUp>();
        Assert.IsNotNull(m_itemScript);
        */
    }

    void Start()
    {
        m_goal = PlayerController.Player;
        Assert.IsNotNull(m_goal);
    }

    void OnEnable()
    {
        m_currentState = State.idle;
        m_rigidbody.useGravity = m_defaultGravity;

        /*
        for (int i = 0; i < m_itemColliders.Count; i++)
        {
            m_itemColliders[i].enabled = true;
            Debug.Log("enabling " + m_itemColliders[i]);
        }*/
    }

    void FixedUpdate()
    {
        switch (m_currentState)
        {
            case State.idle:
                CheckCollision();
                break;

            case State.active:
                // Stop attracting once the item has been picked up
                if (m_item.CurrentState == BasePickUp.State.Disabled)
                {
                    m_currentState = State.finished;
                    //m_rigidbody.isKinematic = true;
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

    // Check for when the player gets close enough
    void CheckCollision()
    {
        if (m_currentState == State.idle)
        {
            // Check if the player is close enough to trigger the attraction            
            if (Vector3.Distance(transform.position, m_goal.transform.position) > m_attractRadius)
            {
                return;
            }

            // Check if there is a wall between the player and the item
            RaycastHit hitInfo;
            if (Physics.Linecast(gameObject.transform.position, m_goal.transform.position
                , out hitInfo, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
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
            StartAttract(AttractMode.pickedup);
        }
    }

    // Bounces the item up before starting to attract
    IEnumerator<float> _Hop()
    {
        // Up being relative to the rotation of the item
        Vector3 newForce = transform.up * m_initalJumpHeight;
        m_rigidbody.AddForce(newForce, ForceMode.VelocityChange);

        m_rigidbody.useGravity = true;

        yield return Timing.WaitForSeconds(m_jumpDelay);

        InitActiveState();
    }
    // Starts a timer before starting to attract
    IEnumerator<float> _Wait()
    {
        yield return Timing.WaitForSeconds(m_jumpDelay);

        InitActiveState();
    }
    // Explodes the item out before starting to attract
    IEnumerator<float> _Explode()
    {
        m_rigidbody.useGravity = true;

        yield return Timing.WaitForSeconds(m_explodeDelay);

        InitActiveState();
    }

    // Prepares the item for attraction before starting to attract
    void InitActiveState()
    {
        /*
        for (int i = 0; i < m_itemColliders.Count; i++)
        {
            m_itemColliders[i].enabled = false;
            //Debug.Log("disabling " + m_itemColliders[i]);
        }*/

        if (m_item != null)
        {
            m_rigidbody.useGravity = false;
            m_currentState = State.active;
        }
        else
        {
            m_currentState = State.finished;
        }
    }

    // Start the attraction process
    public void StartAttract(AttractMode attractMode)
    {
        // Don't do anything if the item has already been picked up
        if (m_item.CurrentState == BasePickUp.State.Disabled)
        {
            return;
        }

        // Disables the item's animations (which prevent movement)
        m_item.Activate();

        if (m_SFX != null)
        {
            AudioPool.Instance.Play(m_SFX, transform.position);
        }

        if (attractMode == AttractMode.pickedup)
        {
            Timing.RunCoroutine(_Hop());
        }
        else if (attractMode == AttractMode.explode)
        {
            Timing.RunCoroutine(_Explode());
        }

        m_currentState = State.starting;
    }

    public void AddForce(Vector3 force)
    {
        m_rigidbody.AddForce(force, ForceMode.VelocityChange);
    }

    // Used to draw how big the attract radius is in the Unity Editor scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, m_attractRadius);
    }
}
