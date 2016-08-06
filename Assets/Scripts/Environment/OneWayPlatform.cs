// Attach to platforms to allow the player to jump through from below
// Allows him to drop through from above if he double taps crouch

using UnityEngine;
using Prime31.MessageKit;
using System.Collections.Generic;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField]
    bool m_allowFallthrough = true;

    List<Collider> m_Colliders = new List<Collider>();
    bool m_playerOnMe = false;

    void Start()
    {
        // Watch for double crouches
        MessageKit.addObserver(MessageTypes.DOUBLETAP_CROUCH, OnDoubleCrouch);

        // Find the collider
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (!colliders[i].isTrigger)
            {
                m_Colliders.Add(colliders[i]);
            }
        }
    }

    void OnDestroy()
    {
        MessageKit.removeObserver(MessageTypes.DOUBLETAP_CROUCH, OnDoubleCrouch);
    }

    void IgnoreColliders(Collider playerCollider, bool ignore)
    {
        for (int i = 0; i < m_Colliders.Count; i++)
        {
            Physics.IgnoreCollision(m_Colliders[i], playerCollider, ignore);
        }
    }

    // If player is coming from below, disable collisions
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            IgnoreColliders(other, true);
        }
    }

    // Re-enable the collisions once the player has come out
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            IgnoreColliders(other, false);
        }
    }

    // Keep track of whether the player is currently touching the platform
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag(Tags.Player))
        {
            m_playerOnMe = true;
        }
    }
    void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag(Tags.Player))
        {
            m_playerOnMe = false;
        }
    }

    // Let the player fall through when he double taps crouch
    void OnDoubleCrouch()
    {
        if (m_playerOnMe && m_allowFallthrough)
        {
            IgnoreColliders(PlayerController.MainCollider, true);
        }
    }
}
