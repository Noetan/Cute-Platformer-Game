using UnityEngine;
using System.Collections;

public class HealthPickup : BasePickUp
{
    public int healthThreshold;
    public int healthGain;
    Health health;

    void Start()
    {
        health = PlayerController.HealthComp;
    }

    protected override void PickUp()
    {
        if (health.currentHealth < healthThreshold)
        {
            health.currentHealth += healthGain;
        }

        base.PickUp();
    }
}