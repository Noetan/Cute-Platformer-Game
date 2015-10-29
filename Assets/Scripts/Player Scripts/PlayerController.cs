// Player Controller script
// Manages the controls of the player and passes the input off to the relevant scripts
// Contains getters for other scripts to use to gain info on the player
// Holds a static reference to our player object

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public static GameObject Player;
    public static Rigidbody RB;
    public static string Tag;
    public static Health HealthComp;
    public static PlayerMove MoveComp;

    #region Inspector Variables
    #endregion

    void Awake()
    {
        Instance = this;
        Player = gameObject;
        RB = Player.GetComponent<Rigidbody>();
        Tag = Player.tag;
        HealthComp = GetComponent<Health>();
        MoveComp = GetComponent<PlayerMove>();
    }

    void Update()
    {
        // Player movement
        if (Input.GetButton("Crouch") && MoveComp.Grounded)
        {
            MoveComp.Crouching = true;
        }
        else
        {
            MoveComp.Crouching = false;
        }
    }

    #region Getters
    public float InputH
    {
        get { return Input.GetAxis("Horizontal"); }
    }
    public float InputV
    {
        get { return Input.GetAxis("Vertical"); }
    }
    #endregion
}
