using UnityEngine;
using System.Collections.Generic;
using MovementEffects;

public class PlayerMelee : MonoBehaviour
{

    // Character scripts
    private PlayerMove m_playerMove;
    private CharacterMotor m_characterMotor;
    private DealDamage m_DoDamage;
    private Sliding m_Sliding;
    private Rigidbody m_rigidBody;
    //This will be used to cycle through 2 different punch animations
    private bool punchleft;
    //This will be used to check if we should ge giving damage.
    private bool Punching;

    // Check if we are diving
    [HideInInspector]
    public bool m_diving = false;
    // Check if we can dive
    private bool m_canDive = true;
    // Check if we have dived since the last time we touched the floor
    private bool m_hasDived = false;

    // Check if we are grounded, using animator
    //private bool m_isGrounded;

    private List<GameObject> BeingPunched = new List<GameObject>();

    //These are our public vars, PunchHitBox should be like the GrabBox,
    //and should encompass the area you want your punch to cover
    public BoxCollider PunchHitBox;

    public int PunchDamage = 1;
    public float PushHeight = 4;
    public float PushForce = 10;

    // Mario-style dive attck - force to use
    public Vector3 diveJumpForce;
    // Multiplier to normal of wall, for repelling force when you dive into a wall
    public float divingWallCrashMultiplier = 150;
    // Layers to ignore when crashing from diving
    public LayerMask wallCrashIgnoreLayers;

    public DealDamage DoDamage
    {
        get
        {
            return m_DoDamage;
        }

        set
        {
            m_DoDamage = value;
        }
    }

    // Use this for initialization
    void Start()
    {
        //We're supposed to be on the same gameobject as the PlayerMove,
        //CharacterMotor etc, so lets get them as reference!
        m_playerMove = GetComponent<PlayerMove>();
        m_characterMotor = GetComponent<CharacterMotor>();
        DoDamage = GetComponent<DealDamage>();
        m_Sliding = GetComponent<Sliding>();
        m_rigidBody = GetComponent<Rigidbody>();

        //Did you even make a PunchBox? Or you were lazy and didn't make one?
        if (!PunchHitBox)
        {
            GameObject GameObjectPunchHitBox = new GameObject();
            PunchHitBox = GameObjectPunchHitBox.AddComponent<BoxCollider>();
            GameObjectPunchHitBox.GetComponent<Collider>().isTrigger = true;
            GameObjectPunchHitBox.transform.parent = transform;
            //Let's place it a little but farther away from us.
            GameObjectPunchHitBox.transform.localPosition = new Vector3(0f, 0f, 1f);
            //It should Ignore Raycast so let's put it on layer 2.
            GameObjectPunchHitBox.layer = 2;
            Debug.LogWarning("You were too lazy to make a PunchHitBox so I made one for you, happy?", GameObjectPunchHitBox);
        }

        //Also lets turn off our PunchHitBox, we'll only turn that on while punching
        //so the Grabbing script doesn't get confused with it.
        //PunchHitBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerMove.Grounded)
        {
            m_canDive = true;
            m_hasDived = false;
        }
        else if (!m_playerMove.Grounded && !m_hasDived)
        {
            m_canDive = true;
        }
        else
        {
            m_canDive = false;
        }

        if (Input.GetButtonDown(Buttons.Jump) && m_diving)
        {
            StopDive();
        }

        if (m_Sliding.m_isSliding)
        {
            return;
        }

        if (Input.GetButtonDown(Buttons.Melee))
        {
            #region reference
            //First off, let's check if we're not already punching, see if the punch animations are playing.
            //if (!CheckIfPlaying("ArmsThrow", 1) && !CheckIfPlaying("ArmsThrow", 1))
            //{
            //If I got here no punch animations are playing so we can punch now :D
            //if (punchleft)
            //{
            //Tell the anim ator to play the left punch, and change the bool so next punch will be right punch!
            //m_playerMove.AnimatorComp.Play("ArmsThrow", 1);
            //punchleft = false;
            //Then start the coroutine so the punch effect happens when the animation already reached the interest part.
            //Timing.RunCoroutine(WaitAndPunch());
            /*}
            else
            {
                playerMove.AnimatorComp.Play("ArmsThrow", 1);
                punchleft = true;
                Timing.RunCoroutine(WaitAndPunch());
            }*/
            //}
            #endregion

            if (m_rigidBody.velocity[0] > 0.1f
                    || m_rigidBody.velocity[2] > 0.1f
                    || m_rigidBody.velocity[0] < -0.1f
                    || m_rigidBody.velocity[2] < -0.1f)
            {
                if (m_canDive)
                {
                    Dive();
                }
            }
            else
            {
                //Tell the anim ator to play the left punch, and change the bool so next punch will be right punch!
                //m_playerMove.ModelAnimator.Play("ArmsThrow", 1);
                punchleft = false;
                //Then start the coroutine so the punch effect happens when the animation already reached the interest part.
                Timing.RunCoroutine(_WaitAndPunch());
            }
        }
    }

    void FixedUpdate()
    {
        //Let's pick the Grounded Bool from the animator, since the player grounded bool is private and we can't get it directly..
        //m_isGrounded = m_playerMove.ModelAnimator.GetBool("Grounded");

        // Check if our slide from dive has stopped, so we can stop diving
        if (!m_Sliding.m_isSliding && m_diving)
        {
            StopDive();
        }
    }

    IEnumerator<float> _WaitAndPunch()
    {
        //Wait for 0.12f time and then punch them in their stupid faces!
        yield return Timing.WaitForSeconds(0.12f);
        PunchThem();
    }

    IEnumerator<float> _WaitAndStopPunch()
    {
        //Wait for 0.1f time before stopping the punch
        yield return Timing.WaitForSeconds(0.1f);
        StopPunch();
    }

    /* Redundant
    //Coroutine for cool waiting stuff
    IEnumerator<float> Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    } */

    // Start punching - if NOT timed Punch, you have to end punching manually
    void PunchThem(bool timedPunch = true)
    {
        //Enable our cool Punching Hitbox to check for enemies in there.
        PunchHitBox.enabled = true;
        //Turn our Punchin bool to true so our TriggerStay will check for people being punched.
        Punching = true;
        if (timedPunch)
        {
            //Start the coroutine that will wait for a moment and stop the punching stuff turning bools back to false.
            Timing.RunCoroutine(_WaitAndStopPunch());
        }
    }

    void StopPunch()
    {
        //Turn stuff back to false so it'll stop checking for people on hitbox
        //PunchHitBox.enabled = false;
        Punching = false;
        //Clear the List of people that got punched on this punch.
        BeingPunched.Clear();
    }

    void Dive()
    {
        // We are now diving
        m_diving = true;
        // We have dived since we last touched the floor
        m_hasDived = true;
        // We cannot dive again mid-dive
        m_canDive = false;
        // Do the dive jump
        m_playerMove.FixedJump(diveJumpForce);
        // Start punching, we will turn off punching ourselves, so it is NOT timed
        PunchThem(false);
        m_playerMove.enabled = false;
        m_Sliding.SetSliding(true);
    }

    void StopDive()
    {
        m_diving = false;
        m_playerMove.enabled = true;
        StopPunch();
    }

    //This function runs for each collider on our trigger zone, on each frame they are on our trigger zone.
    void OnTriggerStay(Collider other)
    {
        //If we're not punching, forget about it, just stop right here!
        if (!Punching)
        {
            return;
        }
        //If we are punching, and the tag on our trigger zone has a RigidBody and it's not tagged Player then...
        if (other.attachedRigidbody && !other.gameObject.CompareTag("Player"))
        {
            //If this guy on our trigger zone is not on our List of people already punched with this punch
            if (!BeingPunched.Contains(other.gameObject))
            {
                //Call the DealDamage script telling it to punch the hell out of this guy
                DoDamage.Attack(other.gameObject, PunchDamage, PushHeight, PushForce);
                //Add him to the list, so we won't hit him again with the same punch.
                BeingPunched.Add(other.gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision col)
    {
        // If we're diving...
        if (m_diving)
        {
            // If it wasn't in our ignore layer...
            if (((1 << col.gameObject.layer) & wallCrashIgnoreLayers) == 0)
            {
                foreach (ContactPoint contact in col.contacts)
                {
                    // If the angle is close to straight up
                    if (Vector3.Angle(contact.normal, Vector3.up) > 80.0f)
                    {
                        // Rebound off the wall
                        Vector3 normal = contact.normal;
                        m_rigidBody.AddForce(normal * divingWallCrashMultiplier);
                    }
                }
            }
        }
    }

    //This will return a TRUE/FALSE on the animation we want to check if is playing.
    bool CheckIfPlaying(string Anim, int Layer)
    {
        //Grabs the AnimatorStateInfo out of our PlayerMove animator for the desired Layer.
        AnimatorStateInfo AnimInfo = PlayerController.AnimatorComp.GetCurrentAnimatorStateInfo(Layer);
        //Returns the bool we want, by checking if the string ANIM given is playing.
        return AnimInfo.IsName(Anim);
    }
}
