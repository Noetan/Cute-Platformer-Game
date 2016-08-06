using UnityEngine;
using System.Collections;

public class InvincibilityPickup : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<InvincibleTrigger>().StartInvincibility();
            Destroy(this.gameObject);
        }
    }

}
