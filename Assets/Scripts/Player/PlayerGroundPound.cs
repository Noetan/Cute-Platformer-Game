using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGroundPound : MonoBehaviour
{

    private PlayerMove playerMove;
    private CharacterMotor characterMotor;
    private DealDamage DoDamage;

    private bool pounding;
    private bool grounded;

    private List<GameObject> BeingPounded = new List<GameObject>();

    [SerializeField]
    BoxCollider PoundHitBox;
    [SerializeField]
    int PoundDamage = 1;
    [SerializeField]
    float PushHeight = 6;
    [SerializeField]
    float PushForce = 12;
    [SerializeField]
    Vector3 PoundForce = new Vector3(0, -20, 0);

    void Start()
    {
        playerMove = GetComponent<PlayerMove>();
        characterMotor = GetComponent<CharacterMotor>();
        DoDamage = GetComponent<DealDamage>();

        PoundHitBox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = playerMove.Grounded;
        if (Input.GetButtonDown("Crouch") && !grounded && !pounding)
        {
            pounding = true;
            StartCoroutine(WaitAndPound());
        }
    }

    IEnumerator WaitAndPound()
    {
        yield return StartCoroutine(Wait(0.12f));
        PoundThem();
    }

    IEnumerator Wait(float duration)
    {
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
            yield return 0;
    }

    void PoundThem()
    {

        PoundHitBox.enabled = true;
        pounding = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        rigidbody.AddRelativeForce(PoundForce, ForceMode.Impulse);
    }

    void OnTriggerStay(Collider other)
    {
        if (!pounding)
        {
            return;
        }
        if (other.attachedRigidbody && other.gameObject.tag != "Player")
        {
            if (!BeingPounded.Contains(other.gameObject))
            {
                DoDamage.Attack(other.gameObject, PoundDamage, PushHeight, PushForce);
                BeingPounded.Add(other.gameObject);
            }
        }
        pounding = false;
        PoundHitBox.enabled = false;
        BeingPounded.Clear();
    }

    //This will return a TRUE/FALSE on the animation we want to check if is playing.
    bool CheckIfPlaying(string Anim, int Layer)
    {
        //Grabs the AnimatorStateInfo out of our PlayerMove animator for the desired Layer.
        AnimatorStateInfo AnimInfo = playerMove.GetComponent<Animator>().GetCurrentAnimatorStateInfo(Layer);
        //Returns the bool we want, by checking if the string ANIM given is playing.
        return AnimInfo.IsName(Anim);
    }
}
