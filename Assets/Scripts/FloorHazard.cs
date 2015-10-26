using UnityEngine;
using System.Collections;

public class FloorHazard : MonoBehaviour
{

    // The Object to move
    public GameObject myHazard;

    // Store original position, and the new MoveTo Position
    private Vector3 HazardOriginalPosition;
    private Vector3 HazardNewPosition;

    // This will control the actual movement on the update function
    private bool Appearing;
    private bool Disappearing;
    // Ensure we don't trigger twice before resetting
    private bool Busy = false;

    // How high to go
    public float NewHazardY = 1f;

    // How long after a player has passed by should we trigger to go up?
    public float TimeToTrigger = 1f;
    // How fast to go up
    public float RaiseSpeed = 0.2f;
    // How much time until we go down
    public float TimeToGoDown = 6f;
    // How fast to go down
    public float GoDownSpeed = 0.2f;

    // How much time until we come back
    public float TimeToTriggerIndependentOfPlayer = 3f;
    // If we want this to be appearing and disappearing independent of player contact.
    public bool PlayerIndependent = false;

    void Start()
    {
        //If you don't have a hazard for this, it's not going to work
        if (!myHazard)
        {
            Debug.Log("No hazard, a hazard object is required to work");
            return;
        }
        // Store the hazard positions when it's down and when its up
        HazardOriginalPosition = myHazard.transform.position;
        HazardNewPosition = HazardOriginalPosition;
        HazardNewPosition.y = NewHazardY;
        // If we're player independent...
        if (PlayerIndependent)
        {
            // Count to trigger
            StartCoroutine(PlayerIndependentTriggering());
        }
    }

    void Update()
    {
        // If we are not appearing nor disappearing, stop here.
        if (!Appearing && !Disappearing)
        {
            return;
        }
        // If there is a Hazard...
        if (myHazard)
        {
            // If I'm appearing
            if (Appearing)
            {
                // Lerp the Y position for smooth movement
                float currentY = Mathf.Lerp(myHazard.transform.position.y, NewHazardY, RaiseSpeed);
                Vector3 NewPosition = new Vector3(myHazard.transform.position.x, currentY, myHazard.transform.position.z);
                myHazard.transform.position = NewPosition;
            }
            // If I'm disappearing
            if (Disappearing)
            {
                float currentY = Mathf.Lerp(myHazard.transform.position.y, HazardOriginalPosition.y, GoDownSpeed);
                Vector3 NewPosition = new Vector3(myHazard.transform.position.x, currentY, myHazard.transform.position.z);
                myHazard.transform.position = NewPosition;
            }
        }
    }

    // This coroutine will wait to trigger then bring the hazard up
    IEnumerator DoTheHazardBringing()
    {
        yield return StartCoroutine(Wait(TimeToTrigger));
        Appearing = true;
        // If the object is coming back
        if (TimeToGoDown > 0)
        {
            StartCoroutine(ComeBack());
        }
        yield return StartCoroutine(Wait(RaiseSpeed * 2f));
        Appearing = false;
    }

    // This one is in case you want this to go on and off constantly without player interaction
    IEnumerator PlayerIndependentTriggering()
    {
        // Wait for the Time To Disappear time, then call the Hazard bringing coroutine.
        yield return StartCoroutine(Wait(TimeToTriggerIndependentOfPlayer));
        yield return StartCoroutine(DoTheHazardBringing());
    }

    IEnumerator ComeBack()
    {
        yield return StartCoroutine(Wait(TimeToGoDown));
        Disappearing = true;
        if (PlayerIndependent)
        {
            StartCoroutine(PlayerIndependentTriggering());
        }
        yield return StartCoroutine(Wait(GoDownSpeed * 2f));
        Disappearing = false;
        // We should know we're not busy anymore, we can trigger again
        Busy = false;
    }

    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }

    void OnTriggerEnter(Collider other)
    {
        // If we're player independent, just exit
        if (PlayerIndependent || Busy)
        {
            return;
        }
        if (other.gameObject.tag == "Player")
        {
            Busy = true;
            StartCoroutine(DoTheHazardBringing());
        }
    }
}