using UnityEngine;
using System.Collections;

public class Climbing : MonoBehaviour {

    private PlayerMove playerMove;
    private CharacterMotor characterMotor;
    private Rigidbody playerRB;
    public float ClimbingSpeed = 10;

    private bool ClimbingRope;
    private bool ClimbingLadder;
    private bool ClimbingWall;
    private bool CanClimb;

    // Use this for initialization
    void Start () {
        playerMove = GetComponent<PlayerMove>();
        characterMotor = GetComponent<CharacterMotor>();
        playerRB = GetComponent<Rigidbody>();

        ClimbingRope = false;
        ClimbingLadder = false;
        ClimbingWall = false;
        CanClimb = true;
    }
	
	// Update is called once per frame
	void Update () {
        if (ClimbingRope)
        {
            //...
            // float h = Input.GetAxisRaw("Horizontal");
            // transform.RotateAround( ropePart.Position, RopePart.up, ClimbingSpeed * Time.deltaTime);
            //...

            if (Input.GetButtonDown("Jump"))
            {
                StopClimbingLadder();
            }
        }
        else if (ClimbingLadder)
        {
            float v = Input.GetAxisRaw("Vertical");
            Vector3 speed = new Vector3(0, v * ClimbingSpeed, 0);
            playerRB.MovePosition(playerRB.position + speed * Time.deltaTime);

            //If we press the jump button we probably don't want to be in the ladder anymore so let's call our stop climbing function
            if (Input.GetButtonDown("Jump"))
            {
                StopClimbingLadder();
            }
        }
        else if (ClimbingWall)
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");
            Vector3 speed = new Vector3(h * ClimbingSpeed, v * ClimbingSpeed, 0);
            playerRB.MovePosition(playerRB.position + speed * Time.deltaTime);

            //If we press the jump button we probably don't want to be in the ladder anymore so let's call our stop climbing function
            if (Input.GetButtonDown("Jump"))
            {
                StopClimbingLadder();
            }
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if (!CanClimb)
        {
            return;
        }

        if (col.gameObject.CompareTag("ClimbableRope"))
        {
            // We are now climbing a rope
            ClimbingRope = true;
            // Disable my Player Move, let me handle the input.
            playerMove.enabled = false;
            // Set the RigidBody to Kinematic as we're handling the movement
            playerRB.isKinematic = true;
            // Set the Bool ClimbingRope on the animnator to true
            //playerMove.animator.SetBool("ClimbingRope", true);
            //...
        }
        else if (col.gameObject.CompareTag("Ladder"))
        {
            // We are now climbing a ladder
            ClimbingLadder = true;
            // Disable my Player Move, let me handle the input.
            playerMove.enabled = false;
            // Set the RigidBody to Kinematic as we're handling the movement
            playerRB.isKinematic = true;
            // Set the Bool ClimbingRope on the animnator to true
            //playerMove.animator.SetBool("ClimbingLadder", true);
        }
        else if (col.gameObject.CompareTag("ClimbableWall"))
        {
            // We are now climbing a wall
            ClimbingWall = true;
            // Disable my Player Move, let me handle the input.
            playerMove.enabled = false;
            // Set the RigidBody to Kinematic as we're handling the movement
            playerRB.isKinematic = true;
            // Set the Bool ClimbingRope on the animnator to true
            //playerMove.animator.SetBool("ClimbingWall", true);
            //...
        }
    }
    
    
    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.CompareTag("ClimbableRope") ||
            col.gameObject.CompareTag("Ladder") ||
            col.gameObject.CompareTag("ClimbableWall"))
        {
            foreach (ContactPoint contact in col.contacts)
            {
                Quaternion rotation = Quaternion.LookRotation(contact.normal);
                transform.rotation = rotation;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("ClimbableRope"))
        {
            StopClimbingRope();
        }
        else if (col.gameObject.tag == "Ladder")
        {
            StopClimbingLadder();
        }
        else if(col.gameObject.CompareTag("ClimbableWall"))
        {
            StopClimbingWall();
        }
    }

    void StopClimbingRope()
    {
        //Let's set the can climb, climbing to false So we're not climbing and we can't climb
        CanClimb = false;
        ClimbingLadder = false;
        //Let's turn on our player movement and uncheck rigidbody kinematic
        //So that gravity will work again on us and we'll be able to control ourselves.
        playerMove.enabled = true;
        playerRB.isKinematic = false;
        //Let's turn off our climbing animation
        //playerMove.animator.SetBool("ClimbingRope", false);
        //And in a couple of F time we'll want to climb agian so we'll start this coroutine
        StartCoroutine(CanClimbAgain());
    }

    void StopClimbingLadder()
    {
        //Let's set the can climb, climbing to false So we're not climbing and we can't climb
        CanClimb = false;
        ClimbingLadder = false;
        //Let's turn on our player movement and uncheck rigidbody kinematic
        //So that gravity will work again on us and we'll be able to control ourselves.
        playerMove.enabled = true;
        playerRB.isKinematic = false;
        //Let's turn off our climbing animation
        //playerMove.animator.SetBool("ClimbingLadder", false);
        //And in a couple of F time we'll want to climb agian so we'll start this coroutine
        StartCoroutine(CanClimbAgain());
    }

    void StopClimbingWall()
    {
        //Let's set the can climb, climbing to false So we're not climbing and we can't climb
        CanClimb = false;
        ClimbingWall = false;
        //Let's turn on our player movement and uncheck rigidbody kinematic
        //So that gravity will work again on us and we'll be able to control ourselves.
        playerMove.enabled = true;
        playerRB.isKinematic = false;
        //Let's turn off our climbing animation
        playerMove.animator.SetBool("ClimbingWall", false);
        //And in a couple of F time we'll want to climb agian so we'll start this coroutine
        StartCoroutine(CanClimbAgain());
    }

    IEnumerator CanClimbAgain()
    {
        yield return StartCoroutine(Wait(0.5f));
        CanClimb = true;
    }

    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }
}
