using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMelee : MonoBehaviour
{

    //We'll use those 3 to communicate with the rest of the kit.
    private PlayerMove playerMove;
    private CharacterMotor characterMotor;
    private DealDamage DoDamage;

    //This will be used to cycle through 2 different punch animations
    private bool punchleft;
    //This will be used to check if we should ge giving damage.
    private bool Punching;

    private List<GameObject> BeingPunched = new List<GameObject>();

    //These are our public vars, PunchHitBox should be like the GrabBox,
    //and should encompass the area you want your punch to cover
    public BoxCollider PunchHitBox;

    public int PunchDamage = 1;
    public float PushHeight = 4;
    public float PushForce = 10;

    // Use this for initialization
    void Start()
    {
        //We're supposed to be on the same gameobject as the PlayerMove,
        //CharacterMotor etc, so lets get them as reference!
        playerMove = GetComponent<PlayerMove>();
        characterMotor = GetComponent<CharacterMotor>();
        DoDamage = GetComponent<DealDamage>();

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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //First off, let's check if we're not already punching, see if the punch animations are playing.
            if (!CheckIfPlaying("ArmsThrow", 1) && !CheckIfPlaying("ArmsThrow", 1))
            {
                //If I got here no punch animations are playing so we can punch now :D
                if (punchleft)
                {
                    //Tell the anim ator to play the left punch, and change the bool so next punch will be right punch!
                    playerMove.animator.Play("ArmsThrow", 1);
                    punchleft = false;
                    //Then start the coroutine so the punch effect happens when the animation already reached the interest part.
                    StartCoroutine(WaitAndPunch());
                }
                else
                {
                    playerMove.animator.Play("ArmsThrow", 1);
                    punchleft = true;
                    StartCoroutine(WaitAndPunch());
                }
            }
        }
    }

    IEnumerator WaitAndPunch()
    {
        //Wait for 0.12f time and then punch them in their stupid faces!
        yield return StartCoroutine(Wait(0.12f));
        PunchThem();
    }

    IEnumerator WaitAndStopPunch()
    {
        //Wait for 0.1f time before stopping the punch
        yield return StartCoroutine(Wait(0.1f));
        StopPunch();
    }

    //Coroutine for cool waiting stuff
    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }

    void PunchThem()
    {
        //Enable our cool Punching Hitbox to check for enemies in there.
        PunchHitBox.enabled = true;
        //Turn our Punchin bool to true so our TriggerStay will check for people being punched.
        Punching = true;
        //Start the coroutine that will wait for a moment and stop the punching stuff turning bools back to false.
        StartCoroutine(WaitAndStopPunch());
    }

    void StopPunch()
    {
        //Turn stuff back to false so it'll stop checking for people on hitbox
        //PunchHitBox.enabled = false;
        Punching = false;
        //Clear the List of people that got punched on this punch.
        BeingPunched.Clear();
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

    //This will return a TRUE/FALSE on the animation we want to check if is playing.
    bool CheckIfPlaying(string Anim, int Layer)
    {
        //Grabs the AnimatorStateInfo out of our PlayerMove animator for the desired Layer.
        AnimatorStateInfo AnimInfo = playerMove.animator.GetCurrentAnimatorStateInfo(Layer);
        //Returns the bool we want, by checking if the string ANIM given is playing.
        return AnimInfo.IsName(Anim);
    }
}
