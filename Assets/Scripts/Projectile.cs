using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    // How much damage the projectile should deal, as well as it's height, force and speed
    [SerializeField]
    int ProjDamage = 1;
    [SerializeField]
    float ProjHeightForce = 2f;
    [SerializeField]
    float ProjForwardForce = 10f;
    [SerializeField]
    float ProjSpeed = 15f;

    // Store the owner of this script so we can't hit ourselves
    public GameObject Owner;

    // How long the projectile will live after being spawned
    [SerializeField]
    float selfDestructTime = 3f;

    // It needs a DealDamage script, so we'll store one here.
    private DealDamage DoDamage;
    // We'll use this bool to check if we have been initiated by the player.
    private bool Initiated = false;

    void Start()
    {
        // Create a Deal Damage Component for us.
        DoDamage = gameObject.AddComponent<DealDamage>();
    }

    // This function will be called from the player to start our projectile.
    public void Initialize()
    {
        Initiated = true;
        // This will start a countdown for it's destroy
        StartCoroutine(selfDestruct(selfDestructTime));
    }

    void FixedUpdate()
    {
        // If it has been initiated by the player
        if (Initiated)
        {
            // Tell the Rigid Body to move forward at the desired speed
            GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().transform.position + (transform.forward * ProjSpeed * Time.deltaTime));
        }
    }

    // This function runs for each collider on our trigger zone, on each frame they are on our trigger zone.
    void OnTriggerEnter(Collider other)
    {
        // If it hasn't been initiated by the player, just stop here.
        if (!Initiated)
        {
            return;
        }
        // Ensure we have an owner to differentiate what we hit
        if (Owner)
        {
            // If the owner and what we hit are the same, ignore
            if (Owner == other.gameObject)
            {
                return;
            }
            // If the owner is an enemy, we can hit the player
            if (Owner.tag == "Enemy" && other.gameObject.tag == "Player" && !other.isTrigger)
            {
                if (other.attachedRigidbody)
                {
                    // Deal our damage
                    DoDamage.Attack(other.gameObject, ProjDamage, ProjHeightForce, ProjForwardForce);
                    Destroy(this.gameObject);
                    return;
                }
            }

        }

        // If the object colliding doesn't have the tag player and is not a trigger
        if (other.gameObject.tag != "Player" && !other.isTrigger)
        {
            // If it's a rigid body, deal the dmage
            if (other.attachedRigidbody)
            {
                DoDamage.Attack(other.gameObject, ProjDamage, ProjHeightForce, ProjForwardForce);
            }
            //If it isn't we still want to destroy the projectile since it has a collider
            Destroy(this.gameObject);
        }
    }

    // Coroutine to wait for the set amount of seconds and destroy itself.
    IEnumerator selfDestruct(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(this.gameObject);
    }
}

