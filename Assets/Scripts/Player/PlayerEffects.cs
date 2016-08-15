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

    // Sound effect settings
    AudioClipSettings landSFXsettings = new AudioClipSettings();

    // Animation states
    int jumpAnimSt;
    int jumpDoubleAnimSt;
    int jumpLongAnimSt;
    // Animation parameters
    int speedAnim;
    int groundAnim;
    int yVelAnim;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();

        Assert.IsNotNull(m_animator);

        MessageKit<PlayerMove.JumpType>.addObserver(MessageTypes.PLAYER_JUMP, OnJump);
        MessageKit.addObserver(MessageTypes.PLAYER_LAND, OnLand);

        landSFXsettings.SingleInstance = true;
        landSFXsettings.OverlapThreshold = 2.0f;

        jumpAnimSt = Animator.StringToHash("Base Layer.Jump");
        jumpDoubleAnimSt = Animator.StringToHash("Base Layer.Jump_Somersault");
        jumpLongAnimSt = Animator.StringToHash("Base Layer.Jump_Long");

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

    #region Message Handlers

    /// Called when the player jumps
    void OnJump(PlayerMove.JumpType jumpType)
    {
        // Sound
        AudioPool.Instance.Play(m_jumpSFX, transform.position);

        switch(jumpType)
        {
            case PlayerMove.JumpType.Normal:
                m_animator.Play(jumpAnimSt, 0);
                PooledDB.Instance.Spawn(Pools.Particles.PlayerJumpGround, PlayerController.Instance.FeetPos, true);
                break;

            case PlayerMove.JumpType.Air:
                PooledDB.Instance.Spawn(Pools.Particles.PlayerJumpAir, PlayerController.Instance.FeetPos, true);
                m_animator.Play(jumpDoubleAnimSt, 0);
                break;

            case PlayerMove.JumpType.Long:
                m_animator.Play(jumpLongAnimSt, 0);
                break;

            case PlayerMove.JumpType.High:
                break;
        }
    }

    /// Called when the player lands on the ground
    void OnLand()
    {
        landSFXsettings.Volume = Mathf.Abs(PlayerController.RB.velocity.y) / m_landSFXDamp;
        AudioPool.Instance.PlayRandom(m_landSFX, transform.position, landSFXsettings);
    }
    #endregion
}
