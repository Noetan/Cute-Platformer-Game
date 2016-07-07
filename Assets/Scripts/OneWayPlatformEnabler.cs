

using UnityEngine;
using System.Collections;

public class OneWayPlatformEnabler : MonoBehaviour
{

    //We need to know which layer number our regular player has
    public int PlayerLayer = 8;
    //Also what layer he should change to to be able to pass through platforms
    public int JumpingUpLayer = 10;
    //Lastly, this is just to know if we should use the Double tap to fall down platforms or not.
    public bool CanDoubleTapDown;

    //We'll use this to communicate with the playerMove.
    private PlayerMove playerMove;

    //We'll use this to know if we should change player layers or just leave it there.
    private bool CanPassThroughPlatforms = false;
    //We need to know if we're falling down or still going up, to choose our layer accordingly
    private float LastY;
    //We'll pick the grounded bool from the Animator and store here to know if we're grounded.
    private bool GroundedBool;

    //To detect a double tap, we'll store time since last tap here.
    private float doubleTapTime;
    // How long we have to double tap
    private float timeBuffer = 0.3f;
    // How many times we have tapped down
    private int tapCount = 0;
    // If we have tapped this frame
    private bool verticalSwitch = false;

    // Use this for initialization
    void Start()
    {
        //Pick the Player Move component
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        //The Update would only be used for the double tapping, so if we don't want it, stop here.
        if (!CanDoubleTapDown)
        {
            return;
        }

        // If we have pressed twice
        if(tapCount == 2)
        {
            // Reset variables
            doubleTapTime = 0;
            tapCount = 0;

            //We should start a coroutine to change our layer so we can fall, then change it back on.
            StartCoroutine(CollisionBackOn());
        }
        // If we have pressed once so far, count the time
        else if (tapCount == 1)
        {
            doubleTapTime += Time.deltaTime;
        }

        // Check if we have pressed crouch, and that we have NOT double tapped yet
        if (Input.GetButtonDown("Crouch") && tapCount < 2)
        {
            // We have pressed down
            verticalSwitch = true;
        }
        // If our time buffer is not up, we must press again to confirm a double tap
        else if (verticalSwitch && doubleTapTime <= timeBuffer)
        {
            if (tapCount == 0) { tapCount = 1; verticalSwitch = false; }
            else if (tapCount == 1) { tapCount = 2; verticalSwitch = false; }
        }

        // We have only pressed once, and our time buffer ran out
        if (tapCount == 1 && doubleTapTime >= timeBuffer)
        {
            // Reset variables
            doubleTapTime = 0;
            tapCount = 0;
        }
    }

    IEnumerator CollisionBackOn()
    {
        //Change our layer to the layer that allows our fall
        playerMove.ModelAnimator.Play("Jump1", 0);
        gameObject.layer = JumpingUpLayer;
        //Wait 0.5f for us to fall
        yield return new WaitForSeconds(0.5f);
        //Then turn it back to the layer that lets us collide with that!
        gameObject.layer = PlayerLayer;
    }

    //This is an udpate that is called less frequently
    void FixedUpdate()
    {
        //Let's pick the Grounded Bool from the animator, since the player grounded bool is private and we can't get it directly..
        //GroundedBool = playerMove.ModelAnimator.GetBool("Grounded");
        GroundedBool = playerMove.Grounded;
        if (!GroundedBool)
        {
            //If my current Y position is less than my Previously recorded Y position, then I'm going down
            if (gameObject.transform.position.y < LastY)
            {
                //So, I'm going down, I should be able to collider with platforms! Change our layer accordingly
                if (CanPassThroughPlatforms)
                {
                    CanPassThroughPlatforms = false;
                    gameObject.layer = PlayerLayer;
                }
            }
            else
            {
                //I'm going up, let's change our layer so I can't collide with platforms.
                if (!CanPassThroughPlatforms)
                {
                    CanPassThroughPlatforms = true;
                    gameObject.layer = JumpingUpLayer;
                }
            }
            //Anyway, lets record the LastY position for a check later.
            LastY = gameObject.transform.position.y;
        }
    }
}


