

using UnityEngine;
using System.Collections;

public class PlayerWallJump : MonoBehaviour
{

    //We'll use those 3 to communicate with the rest of the kit.
    private PlayerMove playerMove;
    private CharacterMotor characterMotor;

    public bool CanWallJump;

    //We'll pick the grounded bool from the Animator and store here to know if we're grounded.
    private bool GroundedBool;
    private GameObject CurrentWall;
    // Use this for initialization
    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        characterMotor = GetComponent<CharacterMotor>();
        CanWallJump = false;

    }

    void Update()
    {
        if (!CanWallJump)
        {
            return;
        }
        if (Input.GetButtonDown("Jump") && !GroundedBool && CanWallJump)
        {

            Vector3 WallPos = CurrentWall.transform.position;
            Vector3 myPos = transform.position;
            myPos.y = 0;
            WallPos.y = 0;
            transform.rotation = Quaternion.LookRotation(myPos - WallPos);

            Vector3 Direction = gameObject.transform.forward;

            Direction.y = 0;
            Direction.x = Direction.x * 15;
            Direction.z = 0;

            Debug.Log("JumpTo " + gameObject.transform.forward + Direction);
            gameObject.GetComponent<Rigidbody>().AddForce(Direction * 50);
            playerMove.Jump(new Vector3(0, 12, 0));
            playerMove.animator.Play("Jump1", 0);
            //And lets set the double jump to false!
            CanWallJump = false;
        }
        if (CanWallJump && !GroundedBool)
        {
            //characterMotor.RotateToDirection(transform.position-CurrentWall.transform.position,900f,true);
            Vector3 WallPos = CurrentWall.transform.position;
            Vector3 myPos = transform.position;
            myPos.y = 0;
            WallPos.y = 0;
            transform.rotation = Quaternion.LookRotation(WallPos - myPos);
        }
    }

    //This is an udpate that is called less frequently
    void FixedUpdate()
    {
        //Let's pick the Grounded Bool from the animator, since the player grounded bool is private and we can't get it directly
        GroundedBool = playerMove.animator.GetBool("Grounded");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            CanWallJump = true;
            CurrentWall = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            CanWallJump = false;
            CurrentWall = null;
        }
    }

}


