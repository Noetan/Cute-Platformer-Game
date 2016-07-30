using UnityEngine;
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
    // Is this spring a trigger or a solid object?
    [SerializeField]
    bool isTrigger = false;
    // Should we set X and Z force to zero (better jump force diretion)
    [SerializeField]
    bool lockPlanarForce = false;
    // How long until this spring is useable again?
    [SerializeField]
    float timeToNextBounce = 0.2f;
    bool isUseable = true;

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

        isUseable = true;
    }

    void OnTriggerEnter(Collider col)
    {
        if (isTrigger)
        {
            BouncedOn();
        }
    }

    public void BouncedOn()
    {
        if (isUseable)
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

                if (lockPlanarForce)
                {
                    m_Rigidbody.velocity = Vector3.zero;
                }
                else
                {
                    m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
                }

                m_Rigidbody.AddForce(finalBounceForce, ForceMode.Impulse);

                if (animController)
                {
                    animController.SetBool("Bounced", true);
                }

                isUseable = false;
                StartCoroutine(WaitAndSetSpring());
            }
            else
            {
                Debug.LogWarning("'Player' tagged object landed on enemy, but without playerMove script attached, is unable to bounce");
            }
        }
    }

    IEnumerator WaitAndSetSpring()
    {
        yield return new WaitForSeconds(timeToNextBounce);
        isUseable = true;
    }
}
