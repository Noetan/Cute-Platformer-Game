﻿using UnityEngine;
using System.Collections;

public class JumpSpring : MonoBehaviour {

    // Force to apply to player when bounced on
    [SerializeField]
    float bounceForce = 25;
    // Sound to make on bounce
    [SerializeField]
    AudioClip bounceSound;
    // Aniator for spring
    [SerializeField]
    Animator animController;

    private GameObject m_player;
    private PlayerMove m_playerMove;
    private Rigidbody m_Rigidbody;

    // Use this for initialization
    void Start () {

        m_player = GameObject.FindGameObjectWithTag("Player");
        m_playerMove = m_player.GetComponent<PlayerMove>();
        m_Rigidbody = m_player.GetComponent<Rigidbody>();        

        // Avoid setup errors
        if (tag != "Spring")
        {
            tag = "Spring";
            Debug.LogWarning("'JumpSpring' script attatched to object without 'Spring' tag, it has been assign automatically", transform);
        }
    }

    public void BouncedOn()
    {
        if (!m_playerMove)
        {
            m_playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        }
        if (bounceSound)
        {
            AudioSource.PlayClipAtPoint(bounceSound, transform.position);
        }
        if (m_playerMove)
        {
            Vector3 finalBounceForce = transform.up;
            finalBounceForce *= bounceForce;

            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
            m_Rigidbody.AddForce(finalBounceForce, ForceMode.Impulse);

            if (animController)
            {
                animController.SetBool("Bounced", true);
            }
        }
        else
        {
            Debug.LogWarning("'Player' tagged object landed on enemy, but without playerMove script attached, is unable to bounce");
        }
    }
}
