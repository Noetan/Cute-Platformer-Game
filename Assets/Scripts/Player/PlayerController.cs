// Player Controller script
// Manages the controls of the player and passes the input off to the relevant scripts
// Contains getters for other scripts to use to gain info on the player
// Holds a static reference to our player object

using UnityEngine;
using Prime31.MessageKit;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public static GameObject Player;
    public static Rigidbody RB;
    public static CapsuleCollider MainCollider;
    public static string Tag;
    static Health HealthComp;
    public static PlayerMove MoveComp;
    public static Animator AnimatorComp;

    #region Inspector Variables
    [SerializeField]
    [Tooltip("How fast the player needs to have clicked the button again to register a double tap. Measured in seconds.")]
    float m_doubleTapTime = 0.3f;
    #endregion

    // Drops
    int m_dropsCount = 0;
    int DROPS_MAX_COUNT = 9999999;

    // Controller events
    int m_crouchPressCount = 0;
    float m_crouchResetTime = 0;

    void Awake()
    {
        Instance = this;
        Player = gameObject;
        RB = Player.GetComponent<Rigidbody>();
        Tag = Player.tag;
        HealthComp = GetComponent<Health>();
        MoveComp = GetComponent<PlayerMove>();
        AnimatorComp = GetComponentInChildren<Animator>();        
        MainCollider = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // Player movement //
        if (Input.GetButton(Buttons.Crouch) && MoveComp.Grounded)
        {
            MoveComp.Crouching = true;
        }
        else
        {
            MoveComp.Crouching = false;
        }

        // Detecting crouch double taps //
        // Increment number of taps and set the reset time
        if (Input.GetButtonDown(Buttons.Crouch))
        {
            m_crouchPressCount++;
            m_crouchResetTime = Time.time + m_doubleTapTime;
        }
        // Fire the doubletapped event
        if (m_crouchPressCount >= 2)
        {
            MessageKit.post(MessageTypes.DOUBLETAP_CROUCH);
            m_crouchPressCount = 0;
        }
        // If the reset time has passed then that means the player never tapped again in time
        if (Time.time > m_crouchResetTime)
        {
            m_crouchPressCount = 0;
        }
    }

    public void AddHealth(int diff)
    {
        int newHp = HealthComp.currentHealth += diff;

        newHp = Mathf.Clamp(newHp, 0, HealthComp.maxHealth);
        HealthComp.currentHealth = newHp;
    }

    public void AddKonpeito(int diff)
    {
        m_dropsCount = Mathf.Clamp(m_dropsCount += diff, 0, DROPS_MAX_COUNT);
    }

    #region Getters
    public float InputH
    {
        get { return Input.GetAxis(Buttons.Horizontal); }
    }

    public float InputV
    {
        get { return Input.GetAxis(Buttons.Vertical); }
    }

    public int CurrentHealth
    {
        get { return HealthComp.currentHealth; }
    }

    public int CurrentKonpeito
    {
        get { return m_dropsCount; }
    }

    public bool Grounded
    {
        get { return MoveComp.Grounded; }
    }
    #endregion
}
