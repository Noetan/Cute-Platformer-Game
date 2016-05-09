using UnityEngine;
using System.Collections;

public class PickUpSpawner : MonoBehaviour 
{
    #region Inspector
    [SerializeField]
    PickUpController.Types m_itemType;

    [SerializeField]
    float m_emitSpeed;
    #endregion

    bool activated = false;

    void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            activated = true;
            Spawn(m_itemType, 1);
        }
    }

    void Spawn(PickUpController.Types type, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Retrieve the pick up from their pool
            GameObject GO = PickUpController.Instance.Spawn(type);
            // Set their starting position 
            GO.transform.position = transform.position;
            GO.SetActive(true);

            AttractedItem goScript = GO.GetComponentInChildren<AttractedItem>();
            goScript.StartAttract(AttractedItem.AttractMode.explode);       



        }
    }
}
