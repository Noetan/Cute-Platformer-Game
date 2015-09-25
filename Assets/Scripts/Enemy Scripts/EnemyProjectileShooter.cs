using UnityEngine;
using System.Collections;

public class EnemyProjectileShooter : MonoBehaviour
{

    private CharacterMotor m_characterMotor;

    // The Projectile to use. It needs the script Projectile on it.
    // If you don't set one, a simple sphere will be used
    [SerializeField]
    GameObject ProjectileObject;
    // Where to spawn. If none, a spot in front of the object will be used
    [SerializeField]
    Transform SpawnPosition;
    // Delay for firing so it isn't immediate. In the future, sync with animations
    [SerializeField]
    float WaitBeforeFiring = 0.2f;
    // Delay before firing again. 
    [SerializeField]
    float CooldownTime = 1f;
    // The distance required to be in to start firing
    [SerializeField]
    float shootingRange = 5f;

    // Store the last fired projectile for set up purposes
    private GameObject LastFiredProjectile;

    private bool CanShoot;

    void Start()
    {
        m_characterMotor = GetComponent<CharacterMotor>();
        CanShoot = true;
    }

    void FixedUpdate()
    {
        if (m_characterMotor.DistanceToTarget > 0.03f && 
            m_characterMotor.DistanceToTarget <= shootingRange && CanShoot)
        {
            StartCoroutine(WaitAndFire(WaitBeforeFiring));
            CanShoot = false;
            StartCoroutine(CoolDown(CooldownTime));
        }
    }

    // Wait the designated length of time before firing
    IEnumerator WaitAndFire(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Vector3 spawnPos = Vector3.zero;

        // If there is no set spawn position, calculate one in front and at roughly half height
        if (!SpawnPosition)
        {
            spawnPos = gameObject.transform.position + gameObject.transform.forward * 2 + Vector3.up * 0.45f;
        }
        else
        {
            spawnPos = SpawnPosition.position;
        }

        // If we have no projectile, use a temporary one
        if (!ProjectileObject)
        {
            LastFiredProjectile = CreateProjectile();
            LastFiredProjectile.transform.rotation = gameObject.transform.rotation;
            LastFiredProjectile.transform.position = spawnPos;
        }
        else
        {
            LastFiredProjectile = GameObject.Instantiate(ProjectileObject, spawnPos, gameObject.transform.rotation) as GameObject;
        }
        // Initialize the spawned Projectile
        LastFiredProjectile.GetComponent<Projectile>().Owner = this.gameObject;
        LastFiredProjectile.GetComponent<Projectile>().Initialize();
    }

    // Wait for the set amount of time before being able to shoot again
    IEnumerator CoolDown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        CanShoot = true;
    }

    // Creates a sphere, used for if there is no set projectile
    GameObject CreateProjectile()
    {
        // Creates a Sphere
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.AddComponent<Projectile>();
        go.AddComponent<SphereCollider>();
        Rigidbody gobody = go.AddComponent<Rigidbody>();
        gobody.isKinematic = true;
        gobody.useGravity = false;
        go.GetComponent<Collider>().isTrigger = true;

        return go;
    }

}