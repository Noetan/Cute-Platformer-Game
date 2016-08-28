// Handles any effects and animations for the player

using UnityEngine;
using UnityEngine.Assertions;
using Prime31.MessageKit;
using MovementEffects;
using System.Collections.Generic;

public class PlayerEffects : MonoBehaviour
{
    #region Inspector
    [Header("Important")]
    [SerializeField]
    Material m_eyeMat;
    [SerializeField]
    Material m_mouthMat;

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

    public enum Eyes
    {
        Normal,
        Squeeze,
        Happy,
        Angry,
        Dead,
        Sad,
        Cry,
        Star,
        Heart,
        Smug,
        Shock,
        Wink,
        Meme
    }
    public enum Mouths
    {
        Normal,
        O,
        Happy,
        Angry,
        Dead,
        Sad,
        Cry,
        Shout,
        Drool,
        Smug,
        Worry,
        Lick,
        Meme
    }

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

    // face control
    Helper.Vec2Int m_faceMatTileCount;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponentInChildren<Animator>();

        Assert.IsNotNull(m_animator);
        Assert.IsNotNull(m_eyeMat);
        Assert.IsNotNull(m_mouthMat);

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

        int rows = Mathf.FloorToInt(1 / m_eyeMat.mainTextureScale.x);
        int columns = Mathf.FloorToInt(1 / m_eyeMat.mainTextureScale.y);
        m_faceMatTileCount = new Helper.Vec2Int(rows, columns);

        Timing.RunCoroutine(_CycleFaceTest());
    }

    void OnDestroy()
    {
        MessageKit<PlayerMove.JumpType>.removeObserver(MessageTypes.PLAYER_JUMP, OnJump);
        MessageKit.removeObserver(MessageTypes.PLAYER_LAND, OnLand);
    }

    void FixedUpdate()
    {
        m_animator.SetFloat(speedAnim, PlayerController.RB.velocity.magnitude);
        m_animator.SetBool(groundAnim, PlayerController.Instance.Grounded);
        m_animator.SetFloat(yVelAnim, PlayerController.RB.velocity.y);        
    }

    public void ChangeFace(Eyes newEye, Mouths newMouth)
    {
        ChangeFace((int)newEye, (int)newMouth);
    }
    public void ChangeFace(int eyeIndex, int mouthIndex)
    {
        m_eyeMat.mainTextureOffset = Helper.GetTexGridOffset(m_eyeMat, m_faceMatTileCount, eyeIndex);
        m_mouthMat.mainTextureOffset = Helper.GetTexGridOffset(m_mouthMat, m_faceMatTileCount, mouthIndex);
    }

#if UNITY_EDITOR
    IEnumerator<float> _CycleFaceTest()
    {
        int i = 0;
        int j = 0;
        while (true)
        {/*
            i = (i + 1) % 13;
            j = (13 - i);*/
            i = (int)Random.Range(0, 12);
            j = (int)Random.Range(0, 12);

            ChangeFace(i, j);
            yield return Timing.WaitForSeconds(0.5f);
        }
    }
#endif

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
