using UnityEngine;
using System.Collections;

public class SurfaceClimbing : MonoBehaviour
{
    private PlayerMove m_playerMove;
    private CharacterMotor m_characterMotor;
    private Rigidbody m_playerRB;

    [SerializeField]
    float ClimbingSpeed = 10;

    private bool ClimbingRope;
    private bool ClimbingLadder;
    private bool ClimbingWall;
    private bool CanClimb;
    private bool StoppingClimbing;
    private Vector3 surfaceNormal;
    private Vector3 surfaceBackwards;
    private Vector3 climbingDirection;

    // Use this for initialization
    void Start()
    {
        m_playerMove = GetComponent<PlayerMove>();
        m_characterMotor = GetComponent<CharacterMotor>();
        m_playerRB = GetComponent<Rigidbody>();

        ClimbingRope = false;
        ClimbingLadder = false;
        ClimbingWall = false;
        CanClimb = true;
        StoppingClimbing = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (ClimbingRope || ClimbingLadder || ClimbingWall)
        {
            Vector3 verticalMovement = Vector3.zero;
            Vector3 lateralMovement = Vector3.zero;
            Vector3 finalMovement = Vector3.zero;
            verticalMovement = climbingDirection.normalized;
            verticalMovement *= Input.GetAxis("Vertical");

            if (Input.GetButtonDown("Jump"))
            {
                StartCoroutine(StopClimbing());
                // Jump in direction opposite of surface...
            }

            // See which type of object we're climbing in case we need specialist behaviour
            if (ClimbingRope)
            {
                //...
            }
            else if (ClimbingLadder)
            {
                //...
            }
            else if (ClimbingWall)
            {
                lateralMovement = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
                lateralMovement = transform.TransformDirection(lateralMovement);
            }

            finalMovement = verticalMovement + lateralMovement;
            m_playerRB.MovePosition(m_playerRB.position + (finalMovement * ClimbingSpeed) * Time.deltaTime);
        }
        else
        {
            ClimbingLadder = false;
            ClimbingRope = false;
            ClimbingWall = false;
        }
    }

    // Get the relative position of an object in respect to another (origin).
    public Vector3 getRelativePosition(Transform origin, Vector3 position)
    {
        Vector3 distance = position - origin.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
        relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
        relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);

        return relativePosition;
    }

    void OnTriggerEnter(Collider col)
    {
        if (!CanClimb)
        {
            return;
        }

        EnterClimbable(col);
    }

    void OnTriggerStay(Collider col)
    {
        if (!CanClimb)
        {
            return;
        }

        if (ClimbingLadder || ClimbingRope || ClimbingWall)
        {
            EnterClimbable(col);
        }
    }

    void EnterClimbable(Collider col)
    {
        if (col.gameObject.CompareTag("Ladder") || col.gameObject.CompareTag("ClimbableRope")
            || col.gameObject.CompareTag("ClimbableWall"))
        {
            // Disable Player Move and set to Kinemtatic, we're handling the movement
            m_playerMove.enabled = false;
            m_playerRB.isKinematic = true;

            // Ladders should always have Z axis facing the player
            surfaceBackwards = col.transform.TransformDirection(-Vector3.forward);
            // Our current forward vector
            Vector3 playerForward = this.transform.TransformDirection(Vector3.forward);

            // Make sure we are looking at the ladder - don't want to grab it if we don't face it
            if (Vector3.Dot(playerForward, surfaceBackwards) > 0.7)
            {
                climbingDirection = col.transform.TransformDirection(Vector3.up);
            }

            // Rotate player to look at ladder
            Quaternion newRotation = Quaternion.FromToRotation(playerForward, surfaceBackwards);
            Vector3 targetDirection = newRotation * playerForward;
            m_characterMotor.RotateToDirection(targetDirection, 10, true);
            
        }

        if (col.CompareTag("Ladder"))
        {
            // Position player to centre on the ladder
            Vector3 posAux = getRelativePosition(col.transform, this.transform.position);
            this.transform.position += transform.right * posAux.x;

            // We are now climbing a ladder
            ClimbingLadder = true;
            // Set the Bool ClimbingRope on the animator to true
            //playerMove.AnimatorComp.SetBool("ClimbingLadder", true);
        }
        else if (col.gameObject.CompareTag("ClimbableRope"))
        {
            ClimbingRope = true;
        }
        else if (col.gameObject.CompareTag("ClimbableWall"))
        {
            ClimbingWall = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Ladder") || col.CompareTag("ClimbableRope")
            || col.CompareTag("ClimbableWall"))
        {
            if (ClimbingLadder || ClimbingRope || ClimbingWall)
            {
                StartCoroutine(StopClimbing());
            }
        }
    }

    IEnumerator StopClimbing()
    {
        if (!StoppingClimbing)
        {
            StoppingClimbing = true;
            ClimbingLadder = false;
            ClimbingRope = false;
            ClimbingWall = false;
            m_playerMove.enabled = true;
            m_playerRB.isKinematic = false;

            CanClimb = false;
            yield return new WaitForSeconds(0.5f);
            CanClimb = true;
            StoppingClimbing = false;
        }
    }
}

