using System;
using UnityEngine;
using UnityEngine.Assertions;
using MemoryManagment;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine.Audio;

public class AudioPool : MonoBehaviour
{
    // Even if these go unused best to leave them in to avoid breaking any references
    public enum MixerGroup
    {
        Master,
        Music,
        SFX,
        Dialogue,
        VoiceClips,
        Ambient,
        Collectibles,
        Tunes,
        Menu
    }

    #region Internals
    [Header("Don't touch")]
    [SerializeField]
    GameObject m_emptyPrefab;
    [SerializeField]
    [Tooltip("Match these with the MixerGroup enum")]
    AudioMixerGroup[] m_mixerGroups;

    [Header("Settings")]
    // How much time must pass before a single instance audioclip can play again
    // Note this ends when the clip stops playing regardless of how high you set this
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_overlapThreshold = 0.1f;

    // A reference to this object
    public static AudioPool Instance { get; set; }
    // The pool which holds all one shot audiosources in the game
    GameObjectPool m_AudioSources;
    // Audio settings set to default values, free to use throughout the codebase
    public static AudioClipSettings DefaultSettings;
    // A list of singleinstance audio clips that are currently playing
    List<AudioSource> m_activeClips = new List<AudioSource>();
    // This constant defines the base part of a semitone
    public const float SEMITONE_BASE = 1.05946f;

    // Use this for initialization
    void Awake()
    {
        Assert.IsNull(Instance, "More than 1 instance of AudioPool detected. ONLY HAVE 1 IN THE SCENE PLZ");        
        Instance = this;

        DefaultSettings = new AudioClipSettings();
        // Set up the object pool of audio sources
        m_AudioSources = new GameObjectPool(1, m_emptyPrefab, this.gameObject);
    }

    IEnumerator<float> StoreClip(CustomBehaviour cb)
    {
        while (cb.GetAudioSource.isPlaying)
        {
            yield return 0f;
        }
        cb.SelfStore();
    }

    /// <summary>
    /// Returns the Mixer Group linked with the enum entry provided
    /// </summary>
    public AudioMixerGroup GetMixerGroup(MixerGroup mg)
    {
        return m_mixerGroups[(int)mg];
    }
    #endregion

    /// <summary>
    /// Plays the given audio clip with a random pitch and volume as defined in the settings given.
    /// </summary>
    public AudioSource PlayRandom(AudioClip clip, Vector3 pos, AudioClipSettings settings)
    {
        if (settings.SingleInstance && m_activeClips.Count > 0)
        {
            // Search the list for the matching audio clip
            for (int i = 0; i < m_activeClips.Count; i++)
            {
                if (m_activeClips[i].clip.GetInstanceID() == clip.GetInstanceID())
                {
                    // Found and overlap period not over yet
                    // End the function and skip the audio clip
                    if (m_activeClips[i].time < m_overlapThreshold && m_activeClips[i].isPlaying)
                    {
                        return null;
                    }
                    // Found and overlap is over
                    else
                    {
                        m_activeClips.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        // Retrieve unused audio source from pool
        GameObject newSound = m_AudioSources.New(pos);
        var cb = newSound.GetComponent<CustomBehaviour>();
        var audSrc = cb.GetAudioSource;
        // Randomization
        float randomPitch = UnityEngine.Random.Range(settings.MinPitch, settings.MaxPitch);
        float randomVol = UnityEngine.Random.Range(settings.MinVolume, settings.MaxVolume);

        newSound.transform.position = pos;
        audSrc.clip = clip;
        audSrc.pitch = randomPitch;
        audSrc.volume = randomVol;
        audSrc.outputAudioMixerGroup = settings.MixerGroup;

        if (settings.SingleInstance)
        {
            m_activeClips.Add(audSrc);
        }

        newSound.SetActive(true);
        Timing.RunCoroutine(StoreClip(cb), Segment.SlowUpdate);

        return audSrc;
    }

    /// <summary>
    /// Plays the given audio clip in the position given.
    /// </summary>
    public AudioSource Play(AudioClip newClip, Vector3 pos)
    {
        return PlayRandom(newClip, pos, DefaultSettings);
    }

    /// <summary>
    /// Plays a random clip from the given clips. With a random pitch and volume as defined in the settings given.
    /// </summary>
    public AudioSource PlayRandom(AudioClip[] clips, Vector3 pos, AudioClipSettings settings)
    {
        if (clips.Length > 0)
        {
            return PlayRandom(clips[UnityEngine.Random.Range(0, clips.Length)], pos, settings);
        }
        else
        {
            Debug.Log("Empty clips array passed into PlayRandom");
            return null;
        }
    }

    /// <summary>
    /// Plays the given audioclip at a random setting based on AudioClipSettings
    /// This clip will then be pitch shifted using the mixergroup by the semitone difference given
    /// Note, make sure your mixergroup has a pitch shifter effect applied
    /// </summary>
    public AudioSource PlayPitchShift(AudioClip clip, Vector3 pos, AudioClipSettings settings
        , float semitoneDiff, string pitchShifter)
    {
        float newPitch = 1 * Mathf.Pow(SEMITONE_BASE, semitoneDiff);
        settings.MixerGroup.audioMixer.SetFloat(pitchShifter, newPitch);
        /*
        float test = 0.0f;
        settings.MixerGroup.audioMixer.GetFloat("CollectiblesPitchShift", out test);
        Debug.Log("set to " + test);*/

        return PlayRandom(clip, pos, settings);
    }

    public AudioSource PlayPitchShift(AudioClip[] clips, Vector3 pos, AudioClipSettings settings
        , float semitoneDiff, string pitchShifter)
    {
        if (clips.Length > 0)
        {
            return PlayPitchShift(clips[UnityEngine.Random.Range(0, clips.Length)], pos, settings, semitoneDiff, pitchShifter);
        }
        else
        {
            Debug.LogWarning("Empty clips array passed into PlayPitchShift");
            return null;
        }
        
    }
}

/// 
/// AudioClipSettings
/// 

[Serializable]
public class AudioClipSettings
{
    [SerializeField]
    AudioPool.MixerGroup m_mixerGroup = AudioPool.MixerGroup.SFX;
    [Header("Randomization")]
    [SerializeField]
    [Range(-3.0f, 3.0f)]
    float m_minPitch = 1.0f;
    [SerializeField]
    [Range(-3.0f, 3.0f)]
    float m_maxPitch = 1.0f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_minVolume = 1.0f;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_maxVolume = 1.0f;
    [SerializeField]
    public bool SingleInstance = false;

    public float MinPitch
    {
        get { return m_minPitch; }
        set
        {
            m_minPitch = Mathf.Clamp(value, -3.0f, 3.0f);
        }
    }

    public float MaxPitch
    {
        get { return m_maxPitch; }
        set
        {
            m_maxPitch = Mathf.Clamp(value, -3.0f, 3.0f);
        }
    }

    public float MinVolume
    {
        get { return m_minVolume; }
        set
        {
            m_minVolume = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }

    public float MaxVolume
    {
        get { return m_maxVolume; }
        set
        {
            m_maxVolume = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }

    public AudioMixerGroup MixerGroup
    {
        get { return AudioPool.Instance.GetMixerGroup(m_mixerGroup); }
    }
}





