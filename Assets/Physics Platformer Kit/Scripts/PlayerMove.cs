using UnityEngine;
using System.Collections;
using MemoryManagment; // Object pools

//handles player movement, utilising the CharacterMotor class
[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(DealDamage))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMove : MonoBehaviour 
{
    #region Inspector
    // setup
    [Header("Setup")]

    // if true, won't apply vertical input
    [SerializeField]
    bool m_sidescroller;
    // floorChecks object. FloorChecks are raycasted down from to check the player is grounded.
    [SerializeField]
    Transform m_floorChecks;
    // object with animation controller on, which you want to animate
    [SerializeField]
    Animator m_animator;
    // play when jumping
    [SerializeField]
    AudioClip m_jumpSound;
    // play when landing on ground	
    [SerializeField]
    AudioClip m_landSound;                 

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
    float m_jumpVelocity =  12f;
	// minimum jump velocity if the player releases the button immediately
    [SerializeField]
    float m_minJumpVel = 6f;
    // how early before hitting the ground you can press jump, and still have it work
    [SerializeField]
    float m_jumpLeniancy = 0.17f;
    // How long you can hold the jump button before you start falling again
    [SerializeField]
    float m_maxJumpTime = 0.25f;

    // Smoke puff that plays on jumps and landing
    [SerializeField]
    GameObject m_jumpParticleEffect;
    // Smoke puff that plays when jumping in the air
    [SerializeField]
    GameObject m_midAirJumpParticleEffect;
    // Where relative to the player should it play
    [SerializeField]
    Transform m_jumpingEffectLocation;

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
    
    Rigidbody m_Rigidbody;
    AudioSource m_AudioSource;
    Collider m_Collider;

    GameObjectPool smokeCircularPool;
    GameObjectPool smokePuffPool;

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

    // setup
    void Awake()
	{
		// create single floorcheck in centre of object, if none are assigned
		if(!m_floorChecks)
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
		if(tag != "Player")
		{
			tag = "Player";
			Debug.LogWarning ("PlayerMove script assigned to object without the tag 'Player', tag has been assigned automatically", transform);
		}
		// usual setup
		m_mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		dealDamage = GetComponent<DealDamage>();
		characterMotor = GetComponent<CharacterMotor>();

        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_Collider = GetComponent<Collider>();

        smokeCircularPool = new GameObjectPool(1, m_jumpParticleEffect);
        smokePuffPool = new GameObjectPool(2, m_midAirJumpParticleEffect);

        // gets child objects of floorcheckers, and puts them in an array
        // later these are used to raycast downward and see if we are on the ground
        floorCheckers = new Transform[m_floorChecks.childCount];
		for (int i=0; i < floorCheckers.Length; i++)
			floorCheckers[i] = m_floorChecks.GetChild(i);

        OriginalMaxAccel = m_accel;
        OriginalMaxSpeed = m_maxSpeed;
	}
	
	// get state of player, values and input
	void Update()
	{	
		// handle jumping
		JumpCalculations ();
		// adjust movement values if we're in the air or on the ground
		curAccel = (grounded) ? m_accel : m_airAccel;
		curDecel = (grounded) ? m_decel : m_airDecel;
		curRotateSpeed = (grounded) ? m_rotateSpeed : airRotateSpeed;

        if (Input.GetButton("Crouch") && grounded)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        m_maxSpeed = (isCrouching) ? m_crouchMaxSpeed : OriginalMaxSpeed;
        m_accel = (isCrouching) ? m_crouchMaxAccel : OriginalMaxAccel;
				
		//get movement axis relative to camera
		screenMovementSpace = Quaternion.Euler (0, m_mainCam.eulerAngles.y, 0);
		screenMovementForward = screenMovementSpace * Vector3.forward;
		screenMovementRight = screenMovementSpace * Vector3.right;
		
		//get movement input, set direction to move in
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		//only apply vertical input to movemement, if player is not sidescroller
		if(!m_sidescroller)
			direction = (screenMovementForward * v) + (screenMovementRight * h);
		else
			direction = Vector3.right * h;
		moveDirection = transform.position + direction;
	}
	
	//apply correct player movement (fixedUpdate for physics calculations)
	void FixedUpdate() 
	{
		//are we grounded
		grounded = IsGrounded ();
		//move, rotate, manage speed
		characterMotor.MoveTo (moveDirection, curAccel, 0.7f, true);
		if (m_rotateSpeed != 0 && direction.magnitude != 0)
			characterMotor.RotateToDirection (moveDirection , curRotateSpeed * 5, true);
		characterMotor.ManageSpeed (curDecel, m_maxSpeed + movingObjSpeed.magnitude, true);
		//set animation values
		if(m_animator)
		{
			m_animator.SetFloat("DistanceToTarget", characterMotor.DistanceToTarget);
			m_animator.SetBool("Grounded", grounded);
			m_animator.SetFloat("YVelocity", m_Rigidbody.velocity.y);
		}
	}
	
	//prevents rigidbody from sliding down slight slopes (read notes in characterMotor class for more info on friction)
	void OnCollisionStay(Collision other)
	{
		//only stop movement on slight slopes if we aren't being touched by anything else
		if (other.collider.tag != "Untagged" || grounded == false)
			return;
		//if no movement should be happening, stop player moving in Z/X axis
		if(direction.magnitude == 0 && slope < m_slopeLimit && m_Rigidbody.velocity.magnitude < 2)
		{
            //it's usually not a good idea to alter a rigidbodies velocity every frame
            //but this is the cleanest way i could think of, and we have a lot of checks beforehand, so it shou
            m_Rigidbody.velocity = Vector3.zero;
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
			if(Physics.Raycast(check.position, Vector3.down, out hit, dist + 0.05f))
			{
				if(!hit.transform.GetComponent<Collider>().isTrigger)
				{
					//slope control
					slope = Vector3.Angle (hit.normal, Vector3.up);
					//slide down slopes
					if(slope > m_slopeLimit && hit.transform.tag != "Pushable")
					{
						Vector3 slide = new Vector3(0f, -m_slideAmount, 0f);
                        m_Rigidbody.AddForce (slide, ForceMode.Force);
					}
					//enemy bouncing
					if (hit.transform.tag == "Enemy" && m_Rigidbody.velocity.y < 0)
					{
						enemyAI = hit.transform.GetComponent<EnemyAI>();
						enemyAI.BouncedOn();
						onEnemyBounce ++;
						dealDamage.Attack(hit.transform.gameObject, 1, 0f, 0f);
					}
                    if (hit.transform.CompareTag("Spring") && m_Rigidbody.velocity.y < 0)
                    {
                        JumpSpring jSpring = hit.transform.GetComponent<JumpSpring>();
                        jSpring.BouncedOn();
                    }
					else
						onEnemyBounce = 0;
					//moving platforms
					if (hit.transform.tag == "MovingPlatform" || hit.transform.tag == "Pushable")
					{
						movingObjSpeed = hit.transform.GetComponent<Rigidbody>().velocity;
						movingObjSpeed.y = 0f;
                        //9.5f is a magic number, if youre not moving properly on platforms, experiment with this number
                        m_Rigidbody.AddForce(movingObjSpeed * m_movingPlatformFriction * Time.fixedDeltaTime, ForceMode.VelocityChange);
					}
					else
					{
						movingObjSpeed = Vector3.zero;
					}
					//yes our feet are on something
					return true;
				}
			}
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
		if(groundedCount < 0.25 && groundedCount != 0 && !m_AudioSource.isPlaying && m_landSound && m_Rigidbody.velocity.y < 1)
		{
			m_AudioSource.volume = Mathf.Abs(m_Rigidbody.velocity.y)/40;
			m_AudioSource.clip = m_landSound;
			m_AudioSource.Play ();
		}
		//if we press jump in the air, save the time
		if (Input.GetButtonDown ("Jump") && !grounded)
			airPressTime = Time.time;
		
		//if were on ground within slope limit
		if (grounded && slope < m_slopeLimit)
		{
			//and we press jump, or we pressed jump just before hitting the ground
			if (Input.GetButtonDown ("Jump") || airPressTime + m_jumpLeniancy > Time.time)
			{
                // Jump!
                if (!isCrouching)
                {
                    StartCoroutine(Jump(m_jumpVelocity, true, false));
                }
                else if (isCrouching && m_Rigidbody.velocity[0] > 0.1f)
                {
                    FixedJump(m_crouchingLongJumpForce);
                }
                else if (isCrouching && m_Rigidbody.velocity[0] <= 0.1f)
                {
                    FixedJump(m_crouchingBackflipJumpForce);
                }
            }
		}
	}
    
    // A variable height jump that goes striaght up
    public IEnumerator Jump(float jumpVelocity, bool smoke, bool midAir = false)
    {
        // 0 out the y velocity so we can double jump at any time
        m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, jumpVelocity, m_Rigidbody.velocity.z);

        // Play smoke puff particle effect
        if (!midAir && smoke)
        {
            GameObject smokeGO = smokeCircularPool.New();
            smokeGO.gameObject.transform.position = m_jumpingEffectLocation.position;
            smokeGO.SetActive(true);
        }
        else if (midAir && smoke)
        {
            GameObject smokeGO = smokePuffPool.New();
            smokeGO.gameObject.transform.position = m_jumpingEffectLocation.position;
            smokeGO.SetActive(true);
        }

        // Play the jump sound
        if (m_jumpSound)
        {
            m_AudioSource.volume = 1;
            m_AudioSource.clip = m_jumpSound;
            m_AudioSource.Play();
        }

        // Wait while the player is still holding jump
        // and the character is still moving up
        while ( Input.GetButton("Jump") && m_Rigidbody.velocity.y >= m_minJumpVel)
        {
            yield return null;
        }
        // If the character is still moving up at this stage
        // then the player let go of the jump button early
        // cut the velocity to make the player fall faster
        if (m_Rigidbody.velocity.y > m_minJumpVel)
        {
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_minJumpVel, m_Rigidbody.velocity.z);
        }  
    }

    // A jump of fixed height/direction
    public void FixedJump(Vector3 JumpForce)
    {
        m_Rigidbody.AddRelativeForce(JumpForce, ForceMode.VelocityChange);
    }

    #region Getters Setters
    public float JumpVelocity
    {
        get { return m_jumpVelocity; }
    }

    public Animator AnimatorComp
    {
        get { return m_animator; }
    }
    public float RotateSpeed
    {
        get { return m_rotateSpeed; }
        set { m_rotateSpeed = value; }
    }
    public Transform JumpingEffectLocation
    {
        get { return m_jumpingEffectLocation; }
    }
    public GameObject JumpingParticleEffect
    {
        get { return m_jumpParticleEffect; }
    }
    #endregion
}