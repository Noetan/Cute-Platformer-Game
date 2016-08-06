using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DealDamage))]
public class InvincibleTrigger : MonoBehaviour
{
    [SerializeField]
    int damageToInflict = 10;
    [SerializeField]
    float hitPushForce = 35f;
    [SerializeField]
    float hitPushHeight = 3f;
    [SerializeField]
    float invincibilityDuration = 10f;
    [SerializeField]
    BoxCollider InvincibilityCollider;

    private DealDamage damage;
    private bool isInvincible = false;
    

    void Awake()
    {
        damage = GetComponent<DealDamage>();

        InvincibilityCollider.enabled = false;
    }

    public void StartInvincibility()
    {
        isInvincible = true;
        InvincibilityCollider.enabled = true;
        StartCoroutine(Wait(invincibilityDuration));
    }

    IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
        InvincibilityCollider.enabled = false;
        isInvincible = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (!isInvincible)
        {
            return;
        }
        if (other.tag == "Enemy" || other.tag == "Pickup")
        {
            if (!other.GetComponent<Health>().flashing)
            {
                damage.Attack(other.gameObject, damageToInflict, hitPushHeight, hitPushForce);
            }
        }
    }
}

