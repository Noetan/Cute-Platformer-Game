

using UnityEngine;
using System.Collections;

public class PlayerWallJump : MonoBehaviour
{

    //We'll use these 3 to communicate with the rest of the kit.
    PlayerMove m_playerMove;
    CharacterMotor m_characterMotor;
    Rigidbody m_rigidBody;
    public bool CanWallJump;

    //We'll pick the grounded bool from the Animator and store here to know if we're grounded.
    bool GroundedBool;
    GameObject CurrentWall;


    // How high we jump off a wall
    public float WallJumpYVelocity = 14;
    // How much we multiply the normal, to force us off the wall
    public float WallJumpHorizontalMultiplier = 750;
    // Store normal of collision so we can use it in calculations
    Vector3 m_collisionNormal;

    // Use this for initialization
    void Start()
    {
        m_playerMove = GetComponent<PlayerMove>();
        m_characterMotor = GetComponent<CharacterMotor>();
        m_rigidBody = GetComponent<Rigidbody>();
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

            /*
            Vector3 Direction = gameObject.transform.forward;

            Direction.y = 0;
            Direction.x = Direction.x * 15;
            Direction.z = 0;

            Debug.Log("JumpTo " + gameObject.transform.forward + Direction);
            gameObject.m_rigidBody.AddForce(Direction * 50);
            playerMove.Jump(new Vector3(0, 12, 0));*/
            // 'Reset' player velocity so we don't get 'super' walljumps/barely any momentum at all
            Vector3 playerVel = m_rigidBody.velocity;
            playerVel = new Vector3(playerVel.x, 0f, playerVel.z);
            m_rigidBody.velocity = playerVel;
            // Ensure for Y component to normal, for the same above reason
            m_collisionNormal = new Vector3(m_collisionNormal.x, 0f, m_collisionNormal.z);
            m_rigidBody.AddForce(m_collisionNormal * WallJumpHorizontalMultiplier);
            StartCoroutine( m_playerMove.Jump(new Vector3(0,WallJumpYVelocity, 0), true, true) );

            m_playerMove.AnimatorComp.Play("Jump1", 0);
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
        GroundedBool = m_playerMove.AnimatorComp.GetBool("Grounded");
    }

    void OnCollisionEnter(Collision col)
    {
        foreach (ContactPoint contact in col.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) > 80.0f)
            {
                CanWallJump = true;
                CurrentWall = col.gameObject;
                m_collisionNormal = contact.normal;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        CanWallJump = false;
        CurrentWall = null;
        m_collisionNormal = Vector3.zero;
    }
}


