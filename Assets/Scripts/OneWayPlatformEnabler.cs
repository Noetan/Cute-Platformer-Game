

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
        //Otherwise lets create a bool for our DoubleTap and set it to false.
        bool doubleTapS = false;

        //If we received a key down S
        if (Input.GetKeyDown(KeyCode.S))
        {
            //Check last time the double tap happened. Was it less than .3f seconds ago?
            if (Time.time < doubleTapTime + .3f)
            {
                //Yes it was!! turn our bool to true.
                doubleTapS = true;
            }
            //Wether it was or wasn't, let's store the time of this tap for further comparison.
            doubleTapTime = Time.time;
        }

        //If our bool was turned to true...
        if (doubleTapS)
        {
            //We should start a coroutine to change our layer so we can fall, then change it back on.
            StartCoroutine(CollisionBackOn());
        }
    }

    IEnumerator CollisionBackOn()
    {
        //Change our layer to the layer that allows our fall
        playerMove.AnimatorComp.Play("Jump1", 0);
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
        GroundedBool = playerMove.AnimatorComp.GetBool("Grounded");
        //GroundedBool = playerMove.OnGround;
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


