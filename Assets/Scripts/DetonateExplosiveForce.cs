using UnityEngine;
using System.Collections;

public class DetonateExplosiveForce : MonoBehaviour
{

    // Radius of explosion
    public float radius = 10f;
    // Force of Explosion
    public float force = 5f;
    // How much extra Y force we want
    public float upwardsBias = 0f;
    // How long until we destroy the gameobject
    public float destroyTimer = 5f;
    // How much health to remove if within radius
    public int damageGiven = 2;

    private Collider[] _colliders;
    private bool hasExploded = false;
    private DealDamage dDamage;

    void Start()
    {
        dDamage = GetComponent<DealDamage>();
    }

    void Update()
    {
        if (!hasExploded)
        {
            Explode();
        }
    }


    private Vector3 _explosionPosition;

    public void Explode()
    {
        if (hasExploded) return;

        _explosionPosition = transform.position;
        _colliders = Physics.OverlapSphere(_explosionPosition, radius);

        foreach (Collider hit in _colliders)
        {
            if (!hit)
            {
                continue;
            }

            if (hit.GetComponent<Rigidbody>())
            {

                hit.GetComponent<Rigidbody>().AddExplosionForce(force, _explosionPosition,
                    radius, upwardsBias, ForceMode.Impulse);
            }

            if (hit.GetComponent<Health>())
            {
                dDamage.Attack(hit.gameObject, damageGiven, 0, 0);
            }
        }
        hasExploded = true;
        StartCoroutine(DestroyExplosion());
    }

    IEnumerator DestroyExplosion()
    {
        yield return StartCoroutine(Wait(destroyTimer));
        Destroy(gameObject);
    }

    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }
}
