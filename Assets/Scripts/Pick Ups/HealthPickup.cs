using UnityEngine;

public class HealthPickup : BasePickUp
{
    [SerializeField]
    int healthGain = 1;

    protected override void PickUp()
    {
        PlayerController.Instance.AddHealth(healthGain);

        base.PickUp();
    }
}