using UnityEngine;
using System.Collections;

public class SimpleCinematicScript : MonoBehaviour
{

    CameraFollow cFollow;
    PlayerMove pMove;

    public Transform Target1;
    public Transform Target2;

    public Vector3 Target1Offset;
    public Vector3 Target2Offset;

    public float Target1FollowSpeed;
    public float Target2FollowSpeed;

    public float DelayStart = 0;
    public float ShowTarget1ThisLong = 0;
    public float ShowTarget2ThisLong = 0;

    private Transform OriginalTarget;
    private Vector3 OriginalTargetOffset;
    private float OriginalTargetFollowSpeed;

    public bool OnlyTriggerOnce = true;

    //Cinematic routine! It'll change our values of the camera follow one at a time
    IEnumerator StartCinematic()
    {
        //Let's wait a delay amount of time to start.
        yield return StartCoroutine(Wait(DelayStart));

        //Let's disable our player movement and set our rigidbody to kinematic so it won't move...
        pMove.enabled = false;
        pMove.gameObject.GetComponent<Rigidbody>().isKinematic = true;

        //Let's set our followspeed, target and target offset to our first cinematic target.
        cFollow.followSpeed = Target1FollowSpeed;
        cFollow.target = Target1;
        cFollow.targetOffset = Target1Offset;

        //wait a bit
        yield return StartCoroutine(Wait(ShowTarget1ThisLong));

        //Set to our second target
        cFollow.followSpeed = Target2FollowSpeed;
        cFollow.target = Target2;
        cFollow.targetOffset = Target2Offset;

        //wait a bit more
        yield return StartCoroutine(Wait(ShowTarget2ThisLong));

        //Reset to the values we had before starting the cinematic.
        cFollow.followSpeed = OriginalTargetFollowSpeed;
        cFollow.target = OriginalTarget;
        cFollow.targetOffset = OriginalTargetOffset;

        //Enable the player movement back, and set the rigid body iskinematic to false so the rigid body will work again.
        pMove.enabled = true;
        pMove.gameObject.GetComponent<Rigidbody>().isKinematic = false;

        //Now, if we want to trigger this again, and not only once...
        if (!OnlyTriggerOnce)
        {
            //We'll wait 5f seconds
            yield return StartCoroutine(Wait(5f));
            //and re-enable the trigger :D
            gameObject.GetComponent<Collider>().enabled = true;
        }
    }

    //This is a fancy wait coroutine for the others to use.
    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }

    //We should trigger if the player enters our trigger
    void OnTriggerEnter(Collider other)
    {
        //If what entered our trigger has the TAG Player...
        if (other.gameObject.CompareTag("Player"))
        {
            //Let's ask the Camera to give us their Camera Follow component to mess with!
            cFollow = Camera.main.GetComponent<CameraFollow>();
            //Let's set the original values that are there right now so we can get back to it later.
            OriginalTarget = cFollow.target;
            OriginalTargetOffset = cFollow.targetOffset;
            OriginalTargetFollowSpeed = cFollow.followSpeed;
            //Also let's set our PlayerMove Script to be the Player that just touched us. So we can disable him.
            pMove = other.gameObject.GetComponent<PlayerMove>();
            //And let's turn this trigger off because... I don't want this being triggered while it's playing the cinematic
            //If that happens I'll be in trouble since I won't have the original values anymore
            gameObject.GetComponent<Collider>().enabled = false;
            //And finally... Let's start the Cinematic!
            StartCoroutine(StartCinematic());
        }
    }
}