public class InvincibilityPickup : BasePickUp
{
    protected override void PickUp()
    {
        PlayerController.Instance.GetComponent<InvincibleTrigger>().StartInvincibility();

        base.PickUp();        
    }
}
