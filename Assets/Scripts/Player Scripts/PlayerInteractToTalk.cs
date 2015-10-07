using UnityEngine;
using System.Collections;
using Fungus; // Remember to add this when using Fungus

public class PlayerInteractToTalk : MonoBehaviour {

    // The flowchart to use, from Fungus
    [SerializeField]
    Flowchart m_flowchart;

    // Turn this off, and probably a lot more things, when speaking
    private PlayerMove m_playerMove;

    // Use this for initialization
    void Start()
    {
        //Pick the Player Move component
        m_playerMove = GetComponent<PlayerMove>();
    }


    void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Talkable") && Input.GetButtonDown("Grab")) // ideally check grounded here too
        {
            m_flowchart.ExecuteBlock(col.GetComponent<NPCIdentifier>().NameForNPC);
        }
    }

    // Set various objects to moving or not
    // Currently only does player, but should do it to projectiles and enemies too
    public void SetSceneMove(bool state)
    {
        if (state)
        {
            m_playerMove.enabled = true;
        }
        else
        {
            m_playerMove.enabled = false;
        }
    }
}


