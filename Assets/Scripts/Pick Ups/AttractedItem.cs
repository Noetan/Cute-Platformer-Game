using UnityEngine;

public class AttractedItem : MonoBehaviour
{
    bool m_active = false;

    GameObject m_player = null;
    Vector3 m_playerPosition = Vector3.zero;
    Rigidbody m_rigidbody;

    [SerializeField]
    float m_attractStrength;

    void Start()
    {
        //m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody = GetComponentInParent<Rigidbody>();
    }

	// Update is called once per frame
	void Update ()
    {
	    if (m_active)
        {
            Vector3 newDir = m_player.transform.position - gameObject.transform.position;
            newDir = newDir.normalized * m_attractStrength;
            m_rigidbody.AddForce(newDir);   
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (!m_active)
        {
            if (other.CompareTag("Player"))
            {
                m_active = true;
                m_player = other.gameObject;
                Debug.Log("attracting");
            }
        }
    }
}
