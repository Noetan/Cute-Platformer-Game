using UnityEngine;
using System.Collections;

public class ExtraJumpEnabler : MonoBehaviour
{

    //This will be used to only allow the double jump if it's before highest jump point. Or not, you choose!
    public bool canAfterHighestJumpPoint = true;

    //We'll use this to communicate with the playerMove.
    private PlayerMove playerMove;

    //This is to keep track if we have double jumped already or if we can double jump      
    private bool canDoubleJump = true;
    //In case of us wanting to only double jump if we're still going up on our jump, we need to store the last Y position.
    private float lastY;
    //We'll pick the grounded bool from the Animator and store here to know if we're grounded.
    private bool isGrounded;
    //We'll store here if we're swimming or not
    private bool isSwimming = false;

    // Use this for initialization
    void Start()
    {
        //Pick the Player Move component
        playerMove = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //If we receive a jump button down, we're not grounded and we can double jump...
        if (Input.GetButtonDown("Jump") && !isGrounded && canDoubleJump)
        {
            //Do a jump with the first jump force! :D
            playerMove.Jump(playerMove.jumpForce);
            //And lets set the double jump to false!
            canDoubleJump = false;
        }
        //If we receive a jump button down, we're not grounded and we are swimming...
        else if (Input.GetButtonDown("Jump") && !isGrounded && isSwimming)
        {
            //Do a jump with the first jump force! :D Or a third of it, because the first one is already too much for swimming
            playerMove.Jump(playerMove.jumpForce / 3);
        }
    }

    void FixedUpdate()
    {
        //Let's pick the Grounded Bool from the animator, since the player grounded bool is private and we can't get it directly..
        isGrounded = playerMove.animator.GetBool("Grounded");
        //If I can't double jump...
        if (!canDoubleJump)
        {
            //But I'm grounded
            if (isGrounded)
            {
                //Then I should turn my Double Jump to on because I can certainly double jump again.
                canDoubleJump = true;
            }
        }
        //If shouldn't be able to double jump after reaching the highest jump point, we'll need this part of the code
        if (!canAfterHighestJumpPoint)
        {
            //If i'm not grounded
            if (!isGrounded)
            {
                //If my current Y position is less than my Previously recorded Y position, then I'm going down
                if (gameObject.transform.position.y < lastY)
                {
                    //So, I'm going down, I shouldn't be able to double jump anymore.
                    canDoubleJump = false;
                }
                //Anyways, lets record the LastY position for a check later.
                lastY = gameObject.transform.position.y;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //If the object has the tag Water that means we're in water now...
        if (other.gameObject.tag == "Water")
        {
            isSwimming = true;

            //If we have an animation for swimming we should set a bool to true or something
            //playerMove.animator.SetBool ("Swimming", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //If a object with the gametag water exited my Trigger...
        if (other.gameObject.tag == "Water")
        {
            isSwimming = false;

            //Let's turn our swimming bool off on the animator
            //playerMove.animator.SetBool ("Swimming", false);
        }
    }
}
