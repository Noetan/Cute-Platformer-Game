using UnityEngine;
using System.Collections;

public class Sliding : MonoBehaviour {

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
    float slideJumpForce = 5f;

    [HideInInspector]
    public bool m_isSliding; 

    private PlayerMove m_playerMove;
    private Rigidbody m_rigidBody;


	// Use this for initialization
	void Start ()
    {
        // Pick the Player Move component
        m_playerMove = GetComponent<PlayerMove>();
        m_rigidBody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (m_isSliding)
        {
            // Do some controls..

            if (Input.GetButtonDown("Jump"))
            {
                SetSliding(false);
                StartCoroutine(m_playerMove.Jump(slideJumpForce, true));
            }
        }
    }

    void FixedUpdate()
    {

        if (m_isSliding)
        {
            // Check X and Z velocities
            // If they're 0, we aren't moving anymore and shouldn't be sliding
            if (m_rigidBody.velocity[0] == 0 &&
                m_rigidBody.velocity[2] == 0)
            {
                SetSliding(false);
            }
        }
        else if (!m_isSliding)
        {

        }

    }

    public void SetSliding(bool isSliding = true)
    {
        m_isSliding = isSliding;

        if (m_isSliding)
        {
            m_playerMove.enabled = false;
            m_playerMove.GetComponent<BoxCollider>().material = m_SlidingMat;
        }
        else
        {
            m_playerMove.enabled = true;
            m_playerMove.GetComponent<BoxCollider>().material = m_DefaultMat;
        }
    }
}
