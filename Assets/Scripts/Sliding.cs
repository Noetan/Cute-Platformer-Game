using UnityEngine;
using UnityEngine.Assertions;
using MovementEffects;
using System.Collections.Generic;

public class Sliding : MonoBehaviour
{

    // TODO: Add sliding controls here (eg moving left and right)
    // - Sliding controls for walking on ice vs sliding down ice slide are different

    // Default Physics Material
    [SerializeField]
    PhysicMaterial m_DefaultMat;
    // Physics Material when sliding
    [SerializeField]
    PhysicMaterial m_SlidingMat;
    // Strength of Mini jump to perform when jumping from Sliding
    [SerializeField]
    float m_slideJumpForce = 5f;
    // How slow does the player need to be before the sliding is disabled
    [SerializeField]
    float m_slideStopEpsilon = 0.1f;

    [HideInInspector]
    public bool IsSliding { get; set; }

    // Cached components
    PlayerMove m_playerMove;
    Rigidbody m_rigidBody;
    Collider m_collider;

    // Use this for initialization
    void Start()
    {
        // Pick the Player Move component
        m_playerMove = GetComponent<PlayerMove>();
        m_rigidBody = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();

        Assert.IsNotNull(m_playerMove);
        Assert.IsNotNull(m_rigidBody);
        Assert.IsNotNull(m_collider);
        Assert.IsFalse(m_collider.isTrigger);
    }

    public void SetSliding(bool isSliding = true)
    {
        IsSliding = isSliding;

        if (IsSliding)
        {
            m_playerMove.enabled = false;
            m_collider.material = m_SlidingMat;
            Timing.RunCoroutine(CheckForStop());
        }
        else
        {
            m_playerMove.enabled = true;
            m_collider.material = m_DefaultMat;
            Debug.Log("sliding stopped");
        }
    }

    // Disable sliding once the player has stopped moving or jumps out
    IEnumerator<float> CheckForStop()
    {
        while (IsSliding)
        {
            // Get the squared magnitude of our current movement speed
            float horizontalSpeed = (m_rigidBody.velocity.x * m_rigidBody.velocity.x)
                + (m_rigidBody.velocity.z * m_rigidBody.velocity.z);
            
            // If the speed is 0, we aren't moving anymore and shouldn't be sliding
            if (horizontalSpeed < m_slideStopEpsilon)
            {
                SetSliding(false);
                break;
            }

            // If the player tries to jump then cancel the sliding
            if (Input.GetButtonDown(Buttons.Jump))
            {
                SetSliding(false);
                StartCoroutine(m_playerMove.Jump(m_slideJumpForce, true));
                break;
            }

            yield return 0f;
        }        
    }
}
