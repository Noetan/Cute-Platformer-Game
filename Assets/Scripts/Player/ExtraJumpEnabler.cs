using UnityEngine;

public class ExtraJumpEnabler : MonoBehaviour
{
    #region Inspector Variables
    //This will be used to only allow the double jump if it's before highest jump point. Or not, you choose!
    [SerializeField]
    bool CanAfterHighestJumpPoint = true;
    // How high we jump off a wall
    [SerializeField]
    float WallJumpYVelocity = 14;
    // How much we multiply the normal, to force us off the wall
    [SerializeField]
    float WallJumpHorizontalMultiplier = 750;
    // How high we jump in midair
    [SerializeField]
    float m_airJumpVel = 12f;
    // Drag when player is 'sliding' down a wall
    [SerializeField]
    float wallSlideDrag = 7.5f;
    // Default drag value;
    float dragDefault;
    [SerializeField]
    float m_WaterJumpStrength = 0.33f;
    #endregion

    //We'll use this to communicate with the playerMove.
    PlayerMove m_playerMove;

    //This is to keep track if we have double jumped already or if we can double jump      
    bool m_canDoubleJump = true;
    //In case of us wanting to only double jump if we're still going up on our jump, we need to store the last Y position.
    float m_lastY;
    //We'll pick the grounded bool from the Animator and store here to know if we're grounded.
    //bool m_isGrounded;
    //We'll store here if we're swimming or not
    bool m_isSwimming = false;
    
    // Store normal of collision so we can use it in calculations
    Vector3 m_collisionNormal;
    GameObject m_currentWall;
    CharacterMotor m_characterMotor;

    [HideInInspector]
    public bool CanWallJump;

    // Reference to gameobject's rigidbody component, set in Start()
    Rigidbody m_rigidBody;

    // Use this for initialization
    void Start()
    {
        //Pick the Player Move component
        m_playerMove = GetComponent<PlayerMove>();
        m_characterMotor = GetComponent<CharacterMotor>();
        m_rigidBody = GetComponent<Rigidbody>();
        CanWallJump = false;

        dragDefault = m_rigidBody.drag;
    }

    // Update is called once per frame
    void Update()
    {
        if(CanWallJump)
        {
            WallJump();
            return;
        }
        else if (m_rigidBody.drag != dragDefault)
        {
            m_rigidBody.drag = dragDefault;
        }

        //If we receive a jump button down, we're not grounded and we can double jump...
        if (Input.GetButtonDown(Buttons.Jump) && !m_playerMove.Grounded && m_canDoubleJump)
        {
            //Do a jump with the first jump force! :D
            StartCoroutine( m_playerMove.Jump(m_airJumpVel, true, true) );
            //And lets set the double jump to false!
            m_canDoubleJump = false;
            //Instantiate(midAirJumpParticleEffect, m_playerMove.JumpingEffectLocation.position, midAirJumpParticleEffect.transform.rotation);
        }
        //If we receive a jump button down, we're not grounded and we are swimming...
        else if (Input.GetButtonDown(Buttons.Jump) && !m_playerMove.Grounded && m_isSwimming)
        {
            //Do a jump with the first jump force! :D Or a third of it, because the first one is already too much for swimming
            StartCoroutine( m_playerMove.Jump(m_playerMove.JumpVelocity * m_WaterJumpStrength, false) );
        }
    }

    void FixedUpdate()
    {
        //Let's pick the Grounded Bool from the animator, since the player grounded bool is private and we can't get it directly..
        //m_isGrounded = m_playerMove.ModelAnimator.GetBool("Grounded");
        //If I can't double jump...
        if (!m_canDoubleJump)
        {
            //But I'm grounded
            if (m_playerMove.Grounded)
            {
                //Then I should turn my Double Jump to on because I can certainly double jump again.
                m_canDoubleJump = true;
            }
        }
        //If shouldn't be able to double jump after reaching the highest jump point, we'll need this part of the code
        if (!CanAfterHighestJumpPoint)
        {
            //If i'm not grounded
            if (!m_playerMove.Grounded)
            {
                //If my current Y position is less than my Previously recorded Y position, then I'm going down
                if (gameObject.transform.position.y < m_lastY)
                {
                    //So, I'm going down, I shouldn't be able to double jump anymore.
                    m_canDoubleJump = false;
                }
                //Anyways, lets record the LastY position for a check later.
                m_lastY = gameObject.transform.position.y;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //If the object has the tag Water that means we're in water now...
        if (other.CompareTag(Tags.Water))
        {
            m_isSwimming = true;

            //If we have an animation for swimming we should set a bool to true or something
            //playerMove.AnimatorComp.SetBool ("Swimming", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        //If a object with the gametag water exited my Trigger...
        if (other.CompareTag(Tags.Water))
        {
            m_isSwimming = false;

            //Let's turn our swimming bool off on the animator
            //playerMove.AnimatorComp.SetBool ("Swimming", false);
        }

        CanWallJump = false;
        m_currentWall = null;
        m_collisionNormal = Vector3.zero;
    }

    void WallJump()
    {
        if (!CanWallJump)
        {
            m_rigidBody.drag = dragDefault;
            return;
        }
        if (Input.GetButtonDown(Buttons.Jump) && !m_playerMove.Grounded && CanWallJump && m_currentWall != null)
        {
            Vector3 WallPos = m_currentWall.transform.position;
            Vector3 myPos = transform.position;
            myPos.y = 0;
            WallPos.y = 0;
            transform.rotation = Quaternion.LookRotation(myPos - WallPos);

            // 'Reset' player velocity so we don't get 'super' walljumps/barely any momentum at all
            Vector3 playerVel = m_rigidBody.velocity;
            playerVel[1] = 0f;
            m_rigidBody.velocity = playerVel;
            // Ensure for Y component to normal, for the same above reason
            m_collisionNormal = new Vector3(m_collisionNormal.x, 0f, m_collisionNormal.z);
            m_rigidBody.AddForce(m_collisionNormal * WallJumpHorizontalMultiplier);
            StartCoroutine( m_playerMove.Jump(WallJumpYVelocity, true, true) );
            
            //And lets set the double jump to false!
            CanWallJump = false;

            //Instantiate(midAirJumpParticleEffect, m_playerMove.JumpingEffectLocation.position, midAirJumpParticleEffect.transform.rotation);
        }
        if (CanWallJump && !m_playerMove.Grounded && m_currentWall != null)
        {
            //characterMotor.RotateToDirection(transform.position-CurrentWall.transform.position,900f,true);
            Vector3 WallPos = m_currentWall.transform.position;
            Vector3 myPos = transform.position;
            myPos.y = 0;
            WallPos.y = 0;
            transform.rotation = Quaternion.LookRotation(WallPos - myPos);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        foreach (ContactPoint contact in col.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) > 80.0f)
            {
                CanWallJump = true;
                m_currentWall = col.gameObject;
                m_collisionNormal = contact.normal;
                m_rigidBody.drag = wallSlideDrag;
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        m_currentWall = null;
        m_rigidBody.drag = dragDefault;
    }
}
