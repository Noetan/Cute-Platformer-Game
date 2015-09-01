using UnityEngine;
using System.Collections;

public class ShrinkAndDestroy : MonoBehaviour {

    // How many seconds to wait before we start shrinking
    public float delayTimer = 4f;
    // How many seconds it takes to rescale object
    public float timeToRescale = 4f;
    // Should we just randomise these values anyway
    public bool randomiseTimers = false;

    // This remains true while it is in the process of scaling.
    private bool isScaling = false;

    void Start()
    {
        if (randomiseTimers)
        {
            delayTimer = Random.Range(1.0f, 7.0f);
            timeToRescale = Random.Range(1.0f, 7.0f);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        StartCoroutine(Shrink());
	}

    IEnumerator Shrink()
    {
        if (isScaling)
        {
            yield break;
        }

        isScaling = true;

        yield return new WaitForSeconds(delayTimer);
        float startTime = Time.time;
        while (Time.time - startTime < timeToRescale)
        {
            float amount = (Time.time - startTime) / timeToRescale;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, amount);
            yield return null;
        }
        Destroy(gameObject);
    }
}
