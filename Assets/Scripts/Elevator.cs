using UnityEngine;
using System.Collections;

public class Elevator : SimpleStateMachine
{
    #region Inspector
    [SerializeField]
    // These elevators normally come in pairs
    // In this case, put them both in the same place and check one of them
    // to be Bottom - this will change it's starting state to Ascend rather than Descend
    bool Bottom;
    [SerializeField]
    float Rise = 6.0f;
    [SerializeField]
    float Horizontal = 3.0f;
    [SerializeField]
    float BoardingHeight = 1.0f;

    [SerializeField]
    float RiseSpeed = 1.0f;
    [SerializeField]
    float SwapSpeed = 2.0f;
    #endregion

    private enum ElevatorStates { Rotate, Descend, Ascend, Swap }

    private Vector3 ascendRoot;
    private Vector3 ascendTarget;
    private Vector3 flipTarget;
    private Vector3 descendRoot;

    private Vector3 flipOrigin;

    private Vector3 forward;

    Rigidbody rb;

    void Start()
    {

        forward = transform.forward;

        ascendRoot = transform.position;
        ascendTarget = ascendRoot + Vector3.up * Rise;
        flipTarget = ascendTarget + forward * Horizontal;
        descendRoot = flipTarget - Vector3.up * Rise;

        flipOrigin = ascendTarget + forward * Horizontal * 0.5f;

        if (Bottom)
        {
            currentState = ElevatorStates.Ascend;
        }
        else
        {
            currentState = ElevatorStates.Descend;
            transform.position = flipTarget;
        }

        rb = GetComponent<Rigidbody>();
    }

    void Ascend_FixedUpdate()
    {

        if (rb.position == ascendTarget)
        {
            currentState = ElevatorStates.Rotate;
            return;
        }

        rb.MovePosition(Vector3.MoveTowards(transform.position, ascendTarget, RiseSpeed * Time.deltaTime));
    }

    float rot;

    void Rotate_EnterState()
    {
        rot = 0;
    }

    void Rotate_FixedUpdate()
    {
        rot += SwapSpeed * Time.deltaTime;

        transform.RotateAround(flipOrigin, transform.right, SwapSpeed * Time.deltaTime);

        if (rot > 180)
        {
            rb.position = flipTarget;
            rb.rotation = Quaternion.identity;
            currentState = ElevatorStates.Descend;
            return;
        }
    }

    void Descend_FixedUpdate()
    {

        if (transform.position == descendRoot)
        {
            currentState = ElevatorStates.Swap;
            return;
        }

        rb.MovePosition(Vector3.MoveTowards(transform.position, descendRoot, RiseSpeed * Time.deltaTime));
    }

    void Swap_FixedUpdate()
    {
        float time = 180 / SwapSpeed;
        float distance = Vector3.Distance(descendRoot, ascendRoot);

        float velocity = distance / time;

        rb.MovePosition(rb.position - (forward * velocity * Time.deltaTime));

        if (Vector3.Distance(descendRoot, transform.position) > Horizontal)
        {
            rb.position = ascendRoot;
            currentState = ElevatorStates.Ascend;
            return;
        }
    }
}
