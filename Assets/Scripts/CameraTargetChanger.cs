

using UnityEngine;
using System.Collections;

public class CameraTargetChanger : MonoBehaviour
{

    CameraFollow cFollow;
    PlayerMove pMove;

    public Transform Target1;

    public Vector3 TargetOffset;

    public float TargetFollowSpeed;

    public bool OnlyTriggerOnce = true;

    public bool TargetIsPlayer = false;

    private Transform OriginalTarget;
    private Vector3 OriginalTargetOffset;
    private float OriginalTargetFollowSpeed;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cFollow = Camera.main.GetComponent<CameraFollow>();

            // Set the original values that are there right now so we can get back to it later.
            OriginalTarget = cFollow.target;
            OriginalTargetOffset = cFollow.targetOffset;
            OriginalTargetFollowSpeed = cFollow.followSpeed;

            cFollow.followSpeed = TargetFollowSpeed;
            if (TargetIsPlayer)
            {
                cFollow.target = other.gameObject.transform;
            }
            else
            {
                cFollow.target = Target1;
            }
            cFollow.targetOffset = TargetOffset;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Reset Camera...
            cFollow = Camera.main.GetComponent<CameraFollow>();

            // Reset to the values we had before starting.
            cFollow.followSpeed = OriginalTargetFollowSpeed;
            cFollow.target = OriginalTarget;
            cFollow.targetOffset = OriginalTargetOffset;

            if (OnlyTriggerOnce)
            {
                gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }
}

