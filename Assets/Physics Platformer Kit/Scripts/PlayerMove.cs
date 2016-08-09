using UnityEngine;
using System.Collections;
using MemoryManagment; // Object pools
using Prime31.MessageKit;

//handles player movement, utilising the CharacterMotor class
[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(DealDamage))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMove : MonoBehaviour
{
    public enum JumpType
    {
        Normal,
        Air,
        Wall,
        Spring,
        Long,
        High
    }

    #region Inspector
    // setup
    [Header("Setup")]

    // if true, won't apply vertical input
    [SerializeField]
    bool m_sidescroller;
    // floorChecks object. FloorChecks are raycasted down from to check the player is grounded.
    [SerializeField]
    Transform m_floorChecks;

    // movement
    [Header("movement")]

    // acceleration/deceleration in air or on the ground
    [SerializeField]
    float m_accel = 70f;
    [SerializeField]
    float m_airAccel = 18f;
    [SerializeField]
    float m_decel = 7.6f;
    [SerializeField]
    float m_airDecel = 1.1f;
    // how fast to rotate on the ground, how fast to rotate in the air
    [SerializeField]
    [Range(0f, 5f)]
    float m_rotateSpeed = 0.7f, airRotateSpeed = 0.4f;
    // maximum speed of movement in X/Z axis	
    [SerializeField]
    float m_maxSpeed = 9;
    // maximum angle of slopes you can walk on, how fast to slide down slopes you can't
    [SerializeField]
    float m_slopeLimit = 40, m_slideAmount = 35;
    // you'll need to tweak this to get the player to stay on moving platforms properly
    [SerializeField]
    float m_movingPlatformFriction = 7.7f;

    // jumping
    [Header("Jumping")]

    // normal jump velocity
    [SerializeField]
    float m_jumpVelocity = 12f;
    // minimum jump velocity if the player releases the button immediately
    [SerializeField]
    float m_minJumpVel = 6f;
    // how early before hitting the ground you can press jump, and still have it work
    [SerializeField]
    float m_jumpLeniancy = 0.17f;
    #endregion

    [HideInInspector]
    public int onEnemyBounce;

    // Reference to the main scene camera
    Transform m_mainCam;

    int onJump;
    bool grounded;
    Transform[] floorCheckers;
    Quaternion screenMovementSpace;
    float airPressTime, groundedCount, curAccel, curDecel, curRotateSpeed, slope;
    Vector3 direction, moveDirection, screenMovementForward, screenMovementRight, movingObjSpeed;

    // Cached components
    CharacterMotor characterMotor;
    EnemyAI enemyAI;
    DealDamage dealDamage;

    Collider m_Collider;

    bool isCrouching = false;
    [SerializeField]
    float m_crouchMaxSpeed = 5f;
    [SerializeField]
    float m_crouchMaxAccel = 35f;
    [SerializeField]
    Vector3 m_crouchingBackflipJumpForce = new Vector3(0f, 15f, -7f);
    [SerializeField]
    Vector3 m_crouchingLongJumpForce = new Vector3(0f, 5f, 15f);
    float OriginalMaxSpeed;
    float OriginalMaxAccel;

    bool canJump = true;



    // setup
    void Awake()
    {
        // create single floorcheck in centre of object, if none are assigned
        if (!m_floorChecks)
        {
            m_floorChecks = new GameObject().transform;
            m_floorChecks.name = "FloorChecks";
            m_floorChecks.parent = transform;
            m_floorChecks.position = transform.position;
            GameObject check = new GameObject();
            check.name = "Check1";
            check.transform.parent = m_floorChecks;
            check.transform.position = transform.position;
            Debug.LogWarning("No 'floorChecks' assigned to PlayerMove script, so a single floorcheck has been created", m_floorChecks);
        }
        // assign player tag if not already
        if (tag != Tags.Player)
        {
            tag = Tags.Player;
            Debug.LogWarning("PlayerMove script assigned to object without the tag 'Player', tag has been assigned automatically", transform);
        }
        // usual setup
        m_mainCam = GameObject.FindGameObjectWithTag(Tags.MainCamera).transform;
        dealDamage = GetComponent<DealDamage>();
        characterMotor = GetComponent<CharacterMotor>();

        m_Collider = GetComponent<Collider>();

        // gets child objects of floorcheckers, and puts them in an array
        // later these are used to raycast downward and see if we are on the ground
        floorCheckers = new Transform[m_floorChecks.childCount];
        for (int i = 0; i < floorCheckers.Length; i++)
            floorCheckers[i] = m_floorChecks.GetChild(i);

        OriginalMaxAccel = m_accel;
        OriginalMaxSpeed = m_maxSpeed;
    }

    // get state of player, values and input
    void Update()
    {
        // handle jumping
        JumpCalculations();
        // adjust movement values if we're in the air or on the ground
        curAccel = (grounded) ? m_accel : m_airAccel;
        curDecel = (grounded) ? m_decel : m_airDecel;
        curRotateSpeed = (grounded) ? m_rotateSpeed : airRotateSpeed;

        m_maxSpeed = (isCrouching) ? m_crouchMaxSpeed : OriginalMaxSpeed;
        m_accel = (isCrouching) ? m_crouchMaxAccel : OriginalMaxAccel;

        // get movement axis relative to camera
        screenMovementSpace = Quaternion.Euler(0, m_mainCam.eulerAngles.y, 0);
        screenMovementForward = screenMovementSpace * Vector3.forward;
        screenMovementRight = screenMovementSpace * Vector3.right;

        // only apply vertical input to movemement, if player is not sidescroller
        if (!m_sidescroller)
            direction = (screenMovementForward * PlayerController.Instance.InputV) + (screenMovementRight * PlayerController.Instance.InputH);
        else
            direction = Vector3.right * PlayerController.Instance.InputH;
        moveDirection = transform.position + direction;
    }

    //apply correct player movement (fixedUpdate for physics calculations)
    void FixedUpdate()
    {
        //are we grounded
        grounded = IsGrounded();
        //move, rotate, manage speed
        characterMotor.MoveTo(moveDirection, curAccel, 0.7f, true);
        if (m_rotateSpeed != 0 && direction.magnitude != 0)
        {
            characterMotor.RotateToDirection(moveDirection, curRotateSpeed * 5, true);
        }
        characterMotor.ManageSpeed(curDecel, m_maxSpeed + movingObjSpeed.magnitude, true);
    }

    //prevents rigidbody from sliding down slight slopes (read notes in characterMotor class for more info on friction)
    void OnCollisionStay(Collision other)
    {
        //only stop movement on slight slopes if we aren't being touched by anything else
        if (!other.collider.CompareTag(Tags.Untagged) || grounded == false)
            return;
        //if no movement should be happening, stop player moving in Z/X axis
        if (direction.magnitude == 0 && slope < m_slopeLimit && PlayerController.RB.velocity.magnitude < 2)
        {
            //it's usually not a good idea to alter a rigidbodies velocity every frame
            //but this is the cleanest way i could think of, and we have a lot of checks beforehand, so it shou
            PlayerController.RB.velocity = Vector3.zero;
        }
    }

    //returns whether we are on the ground or not
    //also: bouncing on enemies, keeping player on moving platforms and slope checking
    private bool IsGrounded()
    {
        //get distance to ground, from centre of collider (where floorcheckers should be)
        float dist = m_Collider.bounds.extents.y;
        //check whats at players feet, at each floorcheckers position
        foreach (Transform check in floorCheckers)
        {
            RaycastHit hit;
            if (Physics.Raycast(check.position, Vector3.down, out hit, dist + 0.05f))
            {
                //Collider[] allColliders = hit.transform.GetComponents<Collider>();

                //for (int i = 0; i < allColliders.Length; i++)
                //{
                //if (!allColliders[i].isTrigger)
                if (!hit.collider.isTrigger)
                {
                    //slope control
                    slope = Vector3.Angle(hit.normal, Vector3.up);
                    //slide down slopes
                    if (slope > m_slopeLimit && !hit.transform.CompareTag(Tags.Pushable))
                    {
                        Vector3 slide = new Vector3(0f, -m_slideAmount, 0f);
                        PlayerController.RB.AddForce(slide, ForceMode.Force);
                    }
                    //enemy bouncing
                    if (hit.transform.CompareTag(Tags.Enemy) && PlayerController.RB.velocity.y < 0)
                    {
                        enemyAI = hit.transform.GetComponent<EnemyAI>();
                        enemyAI.BouncedOn();
                        onEnemyBounce++;
                        dealDamage.Attack(hit.transform.gameObject, 1, 0f, 0f);
                    }

                    if (hit.transform.CompareTag(Tags.Spring) && PlayerController.RB.velocity.y < 0)
                    {
                        JumpSpring jSpring = hit.transform.GetComponent<JumpSpring>();
                        jSpring.BouncedOn();
                    }
                    else
                    {
                        onEnemyBounce = 0;
                    }

                    //moving platforms
                    if (hit.transform.CompareTag(Tags.MovingPlatform) || hit.transform.CompareTag(Tags.Pushable))
                    {
                        movingObjSpeed = hit.rigidbody.velocity;
                        movingObjSpeed.y = 0f;
                        //9.5f is a magic number, if youre not moving properly on platforms, experiment with this number
                        Vector3 newForce = movingObjSpeed * m_movingPlatformFriction * Time.fixedDeltaTime;
                        //Debug.Log(Time.time + " plat " + hit.rigidbody.velocity + " force " + newForce);
                        PlayerController.RB.AddForce(newForce, ForceMode.VelocityChange);
                    }
                    else
                    {
                        movingObjSpeed = Vector3.zero;
                    }

                    // If the player Triggers world events or objects (button pushing, floor buttons, reactions to player etc)
                    if (hit.transform.CompareTag(Tags.Triggerable))
                    {
                        TriggerableObject triggerable = hit.transform.GetComponent<TriggerableObject>();

                        if (triggerable != null)
                        {
                            triggerable.StandingOn(transform.position);
                        }
                    }

                    //yes our feet are on something
                    return true;
                }
            }
            //}
        }
        movingObjSpeed = Vector3.zero;
        //no none of the floorchecks hit anything, we must be in the air (or water)
        return false;
    }

    //jumping
    private void JumpCalculations()
    {
        //keep how long we have been on the ground
        groundedCount = (grounded) ? groundedCount += Time.deltaTime : 0f;

        //play landing sound
        if (groundedCount < 0.25 && groundedCount != 0 && PlayerController.RB.velocity.y < 1)
        {
            MessageKit.post(MessageTypes.PLAYER_LAND);
        }

        //if we press jump in the air, save the time
        if (Input.GetButtonDown(Buttons.Jump) && !grounded)
            airPressTime = Time.time;

        //if were on ground within slope limit
        if (grounded && slope < m_slopeLimit)
        {
            //and we press jump, or we pressed jump just before hitting the ground
            if (Input.GetButtonDown(Buttons.Jump) || airPressTime + m_jumpLeniancy > Time.time)
            {
                // Jump!
                if (!isCrouching)
                {
                    StartCoroutine(Jump(m_jumpVelocity, true, false));
                }
                else if (isCrouching && PlayerController.RB.velocity[0] > 0.1f
                    || isCrouching && PlayerController.RB.velocity[2] > 0.1f
                    || isCrouching && PlayerController.RB.velocity[0] < -0.1f
                    || isCrouching && PlayerController.RB.velocity[2] < -0.1f)
                {
                    FixedJump(m_crouchingLongJumpForce);
                    MessageKit<JumpType>.post(MessageTypes.PLAYER_JUMP, JumpType.Long);
                }
                else if (isCrouching && PlayerController.RB.velocity[0] <= 0.1f
                    || isCrouching && PlayerController.RB.velocity[2] <= 0.1f)
                {
                    FixedJump(m_crouchingBackflipJumpForce);
                    MessageKit<JumpType>.post(MessageTypes.PLAYER_JUMP, JumpType.High);
                }
            }
        }
    }

    // A variable height jump that goes striaght up based on how long the jump button is held for
    public IEnumerator Jump(float jumpVelocity, bool smoke, bool midAir = false)
    {
        // If we can't jump, return without doing anything
        if (!canJump)
        {
            yield break;
        }

        canJump = false;

        if (!midAir)
        {
            MessageKit<JumpType>.post(MessageTypes.PLAYER_JUMP, JumpType.Normal);
        }
        else if (midAir)
        {
            MessageKit<JumpType>.post(MessageTypes.PLAYER_JUMP, JumpType.Air);
        }

        // 0 out the y velocity so we can double jump at any time
        PlayerController.RB.velocity = new Vector3(PlayerController.RB.velocity.x, jumpVelocity, PlayerController.RB.velocity.z);

        // Wait while the player is still holding the jump button
        // and the character is still moving up
        while (Input.GetButton(Buttons.Jump) && PlayerController.RB.velocity.y >= m_minJumpVel)
        {
            yield return null;
        }
        // If the character is still moving up at this stage
        // and the player let go of the jump button early
        // cut the velocity to make the player fall faster
        if (PlayerController.RB.velocity.y > m_minJumpVel)
        {
            PlayerController.RB.velocity = new Vector3(PlayerController.RB.velocity.x, m_minJumpVel, PlayerController.RB.velocity.z);
        }

        // Wait and set canJump to true;
        StartCoroutine(WaitAndSetCanJumpOn());
    }

    // A jump of fixed height/direction
    public void FixedJump(Vector3 jumpForce, bool smoke)
    {
        if (!canJump)
        {
            return;
        }

        canJump = false;

        PlayerController.RB.AddRelativeForce(jumpForce, ForceMode.VelocityChange);

        StartCoroutine(WaitAndSetCanJumpOn());
    }
    public void FixedJump(Vector3 jumpForce)
    {
        FixedJump(jumpForce, false);
    }

    // Wait and set canJump to true;
    // Used to ensure we can't jump multiple times in very short succession,
    // causing 'superjumps'
    IEnumerator WaitAndSetCanJumpOn()
    {
        yield return new WaitForSeconds(0.05f);
        canJump = true;
    }

    #region Getters Setters
    public float JumpVelocity
    {
        get { return m_jumpVelocity; }
    }

    public float RotateSpeed
    {
        get { return m_rotateSpeed; }
        set { m_rotateSpeed = value; }
    }

    public bool Crouching
    {
        get { return isCrouching; }
        set { isCrouching = value; }
    }

    public bool Grounded
    {
        get { return grounded; }
    }
    #endregion
}