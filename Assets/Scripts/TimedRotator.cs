using UnityEngine;
using System.Collections;

public class TimedRotator : MonoBehaviour
{
    private float timeCycle;

    // Total time required to rotate and stop
    [SerializeField]
    float timePerMovement = 3f;
    // Amount to rotate
    [SerializeField]
    Vector3 rotateAmount;

    void Update()
    {
        float time = Time.time;

        if (time > timeCycle)
        {
            timeCycle = time + timePerMovement;
        }

        // Spin in the last second
        if (timeCycle - time <= 1)
        {
            transform.Rotate(rotateAmount * Time.deltaTime, Space.Self);
        }
    }
}
