using UnityEngine;
using System.Collections;

public class PlayerGrabbing : MonoBehaviour {

    // This class is used to help fix the 'Throwing' class problems
    // Needs to be put on the player
    // Old function used the box collider of the player, and it was a miracle it used to work at all,
    // So now this one actually gets the THING YOUR TRYING TO GRAB

    Throwing throwingClassOnPlayer;

    // Use this for initialization
    void Start()
    {
        throwingClassOnPlayer = transform.parent.GetComponent<Throwing>();
    }

    void OnTriggerEnter(Collider other)
    {
        //throwingClassOnPlayer.objectInGrabbox = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        //throwingClassOnPlayer.objectInGrabbox = null;
    }
}
