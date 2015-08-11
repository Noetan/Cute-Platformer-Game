using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour
{
    public AudioClip healthSound;
    public int healthThreshold;
    public int healthGain;
    private Health health;

    void Start()
    {
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            health = other.GetComponent<Health>();
            HeartGet();
        }
    }

    void HeartGet()
    {
        if (healthSound)
        {
            AudioSource.PlayClipAtPoint(healthSound, transform.position);
        }
        Destroy(gameObject);
        if (health.currentHealth < healthThreshold)
        {
            health.currentHealth += healthGain;
        }
    }
}