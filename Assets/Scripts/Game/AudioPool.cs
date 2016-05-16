using System;
using UnityEngine;
using UnityEngine.Assertions;
using MemoryManagment;
using System.Collections.Generic;
using MovementEffects;
using UnityEngine.Audio;

public class AudioPool : MonoBehaviour
{
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
    AudioMixerGroup[] m_mixerGroups;
    [Header("Settings")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_overlapThreshold = 0.8f;

    public static AudioPool Instance { get; set; }
    GameObjectPool m_AudioSources;
    public static AudioClipSettings DefaultSettings;

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
    public void PlayRandom(AudioClip clip, Vector3 pos, AudioClipSettings settings)
    {
        var newSound = m_AudioSources.New(pos);
        var cb = newSound.GetComponent<CustomBehaviour>();
        var audSrc = cb.GetAudioSource;
        float randomPitch = UnityEngine.Random.Range(settings.MinPitch, settings.MaxPitch);
        float randomVol = UnityEngine.Random.Range(settings.MinVolume, settings.MaxVolume);

        newSound.transform.position = pos;
        audSrc.clip = clip;
        audSrc.pitch = randomPitch;
        audSrc.volume = randomVol;
        // If settings doesnt have a mixer assigned then use the Master Mixer
        //audSrc.outputAudioMixerGroup = settings.MixerGroup == null ? m_mixerGroups[0] : settings.MixerGroup;
        audSrc.outputAudioMixerGroup = settings.MixerGroup;

        newSound.SetActive(true);
        Timing.RunCoroutine(StoreClip(cb), Segment.SlowUpdate);
    }

    /// <summary>
    /// Plays the given audio clip in the position given.
    /// </summary>
    public void Play(AudioClip newClip, Vector3 pos)
    {
        PlayRandom(newClip, pos, DefaultSettings);
    }

    /// <summary>
    /// Plays a random clip from the given clips. With a random pitch and volume as defined in the settings given.
    /// </summary>
    public void PlayRandom(AudioClip[] clips, Vector3 pos, AudioClipSettings settings)
    {
        PlayRandom(clips[UnityEngine.Random.Range(0, clips.Length)], pos, settings);
    }
}

[Serializable]
public class AudioClipSettings
{
    [SerializeField]
    AudioPool.MixerGroup m_mixerGroup;
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
            m_minVolume = Mathf.Clamp(value, 0.0f, 10.0f);
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
        get
        {
            return AudioPool.Instance.GetMixerGroup(m_mixerGroup);
        }
    }
}





