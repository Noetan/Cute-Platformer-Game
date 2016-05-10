// Spawn into levels
// 

using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class PickUpSpawner : CustomBehaviour 
{
    #region Inspector
    [Header("Required")]
    [SerializeField]
    PickUpDB.Type m_itemType;

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
        for (int i = 0; i < 100; i++)
        {

        }

        // Prevent this being called when the gameobject is first created
        // As this is called even if the GO is immeditately set inactive
        //if (m_startDone)
        {
            if (m_spawnOnEnable)
            {                
                base.OnEnable();
                Timing.RunCoroutine( _Spawn(m_itemType) );                
            }            
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

    IEnumerator<float> _Spawn(PickUpDB.Type type)
    {
        // transform.rotation points forward, not up as we want
        // so rotate our spawner's rotation back 90 degrees
        Quaternion directionUp = Quaternion.LookRotation(transform.up, -transform.forward);

        // Get random amount
        int amount = Random.Range(m_emitAmountMin, m_emitAmountMax);
        Debug.Log("spawning " + amount + " drops");

        for (int i = 0; i < amount; i++)
        {
            // Retrieve the pick up from their pool
            GameObject GO = PickUpDB.Instance.Spawn(type);
            // Set their starting position 
            GO.transform.position = transform.position;
            GO.SetActive(true);     

            // Get random direction
            Vector3 newPoint = Helper.GetPointOnSphere(directionUp, m_emitAngle);
            Debug.DrawLine(transform.position, transform.position + (newPoint * 5), Color.red, 60);

            // Wait for the pickup to initialize Start()
            yield return 0f;

            AttractedItem goScript = GO.GetComponent<AttractedItem>();

            goScript.AddForce(newPoint * m_emitSpeed);
            goScript.StartAttract(AttractedItem.AttractMode.explode);
        }

        // Store spawner back into pool
        SelfStore();
    }



}
