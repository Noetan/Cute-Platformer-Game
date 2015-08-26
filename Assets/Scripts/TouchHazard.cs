using UnityEngine;
using System.Collections;

public class TouchHazard : MonoBehaviour {

    // Amount of damage to deal to target upon collision
    public int damageToDeal = 1;
    // May want to set this on and off for a particular hazard, won't deal damage if false
    public bool isActive = true;
    // How long to wait before we can damage again
    public float waitAfterDamage = 1.0f;
    // How far away to push on contact
    public float pushForce = 10f;
    // How high to push on contact
    public float pushHeight = 7f;

    private DealDamage dealDamage;

    void Awake () {

        dealDamage = GetComponent<DealDamage>();
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (isActive)
        {
            if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Enemy")
                || col.gameObject.CompareTag("Pickup"))
            {
                dealDamage.Attack(col.gameObject, damageToDeal, pushHeight, pushForce);
                isActive = false;
                StartCoroutine(CanDamageAgain());
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (isActive)
        {
            if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Enemy")
                || col.gameObject.CompareTag("Pickup"))
            {
                dealDamage.Attack(col.gameObject, damageToDeal, pushHeight, pushForce);
                isActive = false;
                StartCoroutine(CanDamageAgain());
            }
        }
    }

    IEnumerator CanDamageAgain()
    {
        yield return StartCoroutine(Wait(waitAfterDamage));
        isActive = true;
    }

    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }
}
