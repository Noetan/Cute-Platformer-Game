using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class PlayerProjectileShooter: MonoBehaviour
{

    private PlayerMove m_playerMove;
    private CharacterMotor m_characterMotor;
    
    // The projectile to use. If none, a simple sphere will be created
    [SerializeField]
    GameObject ProjectileObject;
    // Where to spawn projectile. If none, a temporary one will be calculated
    [SerializeField]
    Transform SpawnPosition;
    // Delay before firing, to sync up animations
    [SerializeField]
    float WaitBeforeFiring = 0.2f;
    // Delay before firing again
    [SerializeField]
    float CooldownTime = 1f;

    // Store last projectile for setup purposes
    private GameObject LastFiredProjectile;

    private bool CanShoot;


    void Start()
    {
        m_playerMove = GetComponent<PlayerMove>();
        CanShoot = true;
    }

    void Update()
    {
        // Check if we pressed our Fire key, and that we can shoot
        if (Input.GetAxis("Fire") >0 && CanShoot)
        {
            // Check if we're not already playing our animation.
            // Can't use right now...
            //if (!CheckIfPlaying("ArmsThrow", 1))
            //{
                // Turn on animation
                //m_playerMove.AnimatorComp.Play("ArmsThrow", 1);
                // Wait for the set delay and then fire the projectile
                Timing.RunCoroutine(WaitAndFire(WaitBeforeFiring));
                // Start the cooldown
                CanShoot = false;
                Timing.RunCoroutine(CoolDown(CooldownTime));
            //}
        }
    }

    // Enumerator to wait the set delay, and then fire the projectile 
    IEnumerator<float> WaitAndFire(float waitTime)
    {
        yield return Timing.WaitForSeconds(waitTime);

        Vector3 spawnPos = Vector3.zero;

        // If no spawn position is set, calculate one in front at roughly half height
        if (!SpawnPosition)
        {
            spawnPos = gameObject.transform.position + gameObject.transform.forward + Vector3.up * 0.45f;
        }
        else
        {
            spawnPos = SpawnPosition.position;
        }

        // If no projectile prefab, make a simple sphere and initialise
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
        // Initialise setup for the projectile
        LastFiredProjectile.GetComponent<Projectile>().Initialize();
    }

    // Cool down delay to stop projectile spam
    IEnumerator<float> CoolDown(float waitTime)
    {
        yield return Timing.WaitForSeconds(waitTime);
        CanShoot = true;
    }

    //Creates a sphere, used when no projectile is set
    GameObject CreateProjectile()
    {
        //Creates a Sphere
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.AddComponent<Projectile>();
        go.AddComponent<SphereCollider>();
        Rigidbody gobody = go.AddComponent<Rigidbody>();
        gobody.isKinematic = true;
        gobody.useGravity = false;
        go.GetComponent<Collider>().isTrigger = true;

        return go;
    }

    bool CheckIfPlaying(string Anim, int Layer)
    {
        // Grabs the AnimatorStateInfo out of PlayerMove animator for the desired Layer.
        AnimatorStateInfo AnimInfo = m_playerMove.AnimatorComp.GetCurrentAnimatorStateInfo(Layer);
        // Returns the bool we want, by checking if the string ANIM given is playing.
        return AnimInfo.IsName(Anim);
    }
}