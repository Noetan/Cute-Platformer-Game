using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    //We'll need to turn off and on our renderer for this
    Renderer PlatformMesh;

    //How much time until we disappear?
    public float TimeToDisappear = 1f;
    //How much time until we come back?
    public float TimeToComeBack = 6f;

    //How much time until we come back?
    public float TimeToDisappearIndependentOfPlayer = 3f;
    //If we want this to be appearing and disappearing independent of player contact.
    public bool PlayerIndependent = false;

    //It's the start of the game!
    void Start()
    {
        //Let's get a reference to our renderer so we can turn it on and off
        PlatformMesh = GetComponent<Renderer>();
        //If we're player independent...
        if (PlayerIndependent)
        {
            //We should start counting to disappear
            StartCoroutine(PlayerIndependentDisappearing());
        }
    }

    //This coroutine will do the blinking over time
    IEnumerator DoTheBlinking()
    {
        //Start false, wait for X time, than go true, and then false, and...
        PlatformMesh.enabled = false;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.1f));
        PlatformMesh.enabled = true;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.3f));
        PlatformMesh.enabled = false;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.2f));
        PlatformMesh.enabled = true;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.1f));
        PlatformMesh.enabled = false;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.05f));
        PlatformMesh.enabled = true;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.025f));
        PlatformMesh.enabled = false;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.02f));
        PlatformMesh.enabled = true;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.01f));
        PlatformMesh.enabled = false;
        //Now that we're finally gone, turn collider off
        gameObject.GetComponent<Collider>().enabled = false;
        //If the developer wants it to come back (TimeToComeBack bigger than 0...)
        if (TimeToComeBack > 0)
        {
            //Let's start our ComeBack Routine
            StartCoroutine(ComeBack());
        }
    }

    //This one is in case you want this to go on and off constantly without player interaction
    IEnumerator PlayerIndependentDisappearing()
    {
        //It'll wait for the Time To Disappear time, than call the DoTheBlinking coroutine.
        yield return StartCoroutine(Wait(TimeToDisappearIndependentOfPlayer));
        yield return StartCoroutine(DoTheBlinking());
    }

    //Come back routine! Like the disappearing one but opposite!
    IEnumerator ComeBack()
    {
        //It'll wait for the Time To ComeBack time, than blink a bit.
        yield return StartCoroutine(Wait(TimeToComeBack));
        yield return StartCoroutine(Wait(TimeToDisappear * 0.1f));
        PlatformMesh.enabled = true;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.3f));
        PlatformMesh.enabled = false;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.2f));
        PlatformMesh.enabled = true;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.1f));
        PlatformMesh.enabled = false;
        yield return StartCoroutine(Wait(TimeToDisappear * 0.05f));
        PlatformMesh.enabled = true;
        //And now we enable our collider again.
        gameObject.GetComponent<Collider>().enabled = true;
        //If we're player independent...
        if (PlayerIndependent)
        {
            //We should start counting to disappear again.
            StartCoroutine(PlayerIndependentDisappearing());
        }
    }

    //This is a fancy wait coroutine for the others to use.
    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }

    //We should blink and disappear if the player collides with us
    void OnCollisionEnter(Collision other)
    {
        //If we're player independent, just stop this D:
        if (PlayerIndependent)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DoTheBlinking());
        }
    }
}
