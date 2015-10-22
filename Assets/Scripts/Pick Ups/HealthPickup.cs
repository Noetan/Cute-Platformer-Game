using UnityEngine;
using System.Collections;

public class HealthPickup : BasePickUp
{
    public int healthThreshold;
    public int healthGain;
    Health health;

    void Start()
    {
        //health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        health = PlayerController.HealthComp;
    }

    /*
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerController.Tag))
        {
            //health = other.GetComponent<Health>();
        }

        base.OnTriggerEnter(other);
    }*/

    protected override void PickUp()
    {
        if (health.currentHealth < healthThreshold)
        {
            health.currentHealth += healthGain;
        }

        base.PickUp();
    }
}