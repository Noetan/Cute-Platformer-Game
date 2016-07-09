// Handles any effects and animations for the player

using UnityEngine;
using UnityEngine.Assertions;
using Prime31.MessageKit;

public class PlayerEffects : MonoBehaviour
{
    #region
    [Header("Sound")]
    // play when jumping
    [SerializeField]
    AudioClip m_jumpSFX;
    // play when landing on ground	
    [SerializeField]
    AudioClip m_landSFX;
    [Tooltip("Dampening factor used with fall speed to determine land SFX volume")]
    [SerializeField]
    float m_landSFXDamp = 40f;
    #endregion

    Animator m_animator;
    float feetOffset = 0f;

    // Sound effect settings
    AudioClipSettings landSFXsettings = new AudioClipSettings();

    // Animation states
    int jumpAnimSt;
    // Animation parameters
    int speedAnim;
    int groundAnim;
    int yVelAnim;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        feetOffset = GetComponent<CapsuleCollider>().height / 2;

        Assert.IsNotNull(m_animator);

        MessageKit<PlayerMove.JumpType>.addObserver((int)Constants.Messages.PlayerJump, OnJump);
        MessageKit.addObserver((int)Constants.Messages.PlayerLand, OnLand);

        landSFXsettings.SingleInstance = true;
        landSFXsettings.OverlapThreshold = 2.0f;

        jumpAnimSt = Animator.StringToHash("Jump.Jump_StandToUp");

        speedAnim = Animator.StringToHash("Speed");
        groundAnim = Animator.StringToHash("Grounded");
        yVelAnim = Animator.StringToHash("YVelocity");
    }

    void FixedUpdate()
    {
        m_animator.SetFloat(speedAnim, PlayerController.RB.velocity.magnitude);
        m_animator.SetBool(groundAnim, PlayerController.Instance.Grounded);
        m_animator.SetFloat(yVelAnim, PlayerController.RB.velocity.y);
    }

    void OnJump(PlayerMove.JumpType jumpType)
    {
        // Animation
        m_animator.Play(jumpAnimSt, 0);

        // Sound
        AudioPool.Instance.Play(m_jumpSFX, transform.position);

        // Particle effects
        Vector3 effectPos = transform.position - new Vector3(0, feetOffset, 0);

        if (jumpType == PlayerMove.JumpType.Normal)
        {
            PooledDB.Instance.Spawn(PooledDB.Particle.PlayerJumpGround, effectPos, true);
        }
        else if (jumpType == PlayerMove.JumpType.Air)
        {
            PooledDB.Instance.Spawn(PooledDB.Particle.PlayerJumpAir, effectPos, true);
        }
    }

    void OnLand()
    {
        landSFXsettings.Volume = Mathf.Abs(PlayerController.RB.velocity.y) / m_landSFXDamp;
        AudioPool.Instance.PlayRandom(m_landSFX, transform.position, landSFXsettings);
    }
}
