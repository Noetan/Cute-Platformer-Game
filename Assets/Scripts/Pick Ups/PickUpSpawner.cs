// Spews drops out at this location
// Meant to be spawned in-game and will automatically trigger attracted items

// Can either be set to spawn on enable (i.e. spawning it into a broken crate or dead enemy)
// Or can spawn when the player touches its trigger collider

// Will destroy/store itself once it's done spawning

using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class PickUpSpawner : CustomBehaviour
{
    #region Inspector
    [Header("Required")]
    [SerializeField]
    PooledDB.PickUp m_itemType;

    [Header("Spawn properties")]
    [SerializeField]
    float m_emitSpeed = 150f;
    [SerializeField]
    float m_emitAngle = 75f;
    [SerializeField]
    int m_emitAmountMin = 50;
    [SerializeField]
    int m_emitAmountMax = 50;

    [SerializeField]
    bool m_spawnOnEnable = true;
    #endregion

    // Prevent the spawner getting triggered multiple times
    bool finished = false;

    protected override void OnEnable()
    {
        // Prevent this being called when the gameobject is first created
        // As this is called even if the GO is immeditately set inactive

        finished = false;

        if (m_spawnOnEnable)
        {
            base.OnEnable();
            Timing.RunCoroutine(_Spawn(m_itemType));
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (!finished && other.CompareTag("Player"))
        {
            finished = true;
            Timing.RunCoroutine(_Spawn(m_itemType));
        }
    }

    IEnumerator<float> _Spawn(PooledDB.PickUp type)
    {
        // transform.rotation points forward, not up as we want
        // so rotate our spawner's rotation back 90 degrees
        var directionUp = Quaternion.LookRotation(transform.up, -transform.forward);

        // Get random amount
        int amount = Random.Range(m_emitAmountMin, m_emitAmountMax);

        for (int i = 0; i < amount; i++)
        {
            // Make sure Start() is initialized before continuing
            while (!m_startDone)
            {
                yield return 0f;
            }

            // Retrieve the pick up from their pool
            var GO = PooledDB.Instance.Spawn(type, transform.position, true);

            // Get random direction
            var newPoint = Helper.GetPointOnSphere(directionUp, m_emitAngle);
            Debug.DrawLine(transform.position, transform.position + (newPoint * 5), Color.red, 60);

            // Wait for the pickup to initialize Start()
            yield return 0f;

            var goScript = GO.GetComponent<AttractedItem>();

            goScript.AddForce(newPoint * m_emitSpeed);
            goScript.StartAttract(AttractedItem.AttractMode.explode);
        }

        // Store spawner back into pool
        SelfStore();
    }
}
