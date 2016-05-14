using System;
using UnityEngine;
using UnityEngine.Assertions;
using MemoryManagment;
using System.Collections.Generic;
using MovementEffects;

public class AudioPool : MonoBehaviour
{
    // Add your entry here. Then assign the audio file in the inspector of this object.
    public enum Bank
    {
        DropPickUp,
        Jump
    }

    #region Internals
    [Serializable]
    public class Clip
    {
        public AudioClip[] ClipFile;
        public bool SingleInstance = false;
        [HideInInspector]
        public bool Running = false;

        int currentClip = 0;

        public AudioClip GetNextClip()
        {
            return ClipFile[(currentClip++) % ClipFile.Length];
        }
    }

    // The audio files that match up with each Bank entry
    [SerializeField]
    Clip[] m_AudioFiles = new Clip[Enum.GetValues(typeof(Bank)).Length];
    [SerializeField]
    GameObject m_emptyPrefab;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    float m_overlapThreshold = 0.8f;

    public static AudioPool Instance { get; set; }
    GameObjectPool m_AudioSources;

    // Use this for initialization
    void Awake()
    {
        Assert.IsNull(Instance, "More than 1 instance of AudioPool detected. ONLY HAVE 1 IN THE SCENE PLZ");
        Instance = this;

        Assert.AreEqual(m_AudioFiles.Length, Helper.CountEnum(typeof(Bank)));

        // Set up the object pool of audio sources
        m_AudioSources = new GameObjectPool(1, m_emptyPrefab, this.gameObject);        
    }

    IEnumerator<float> StoreClip(CustomBehaviour cb, Clip clip)
    {
        float overlapDelay = cb.GetAudioSource.clip.length * m_overlapThreshold;

        yield return Timing.WaitForSeconds(overlapDelay);
        clip.Running = false;

        while (cb.GetAudioSource.isPlaying)
        {
            yield return 0f;
        }
        cb.SelfStore();
    }
    #endregion

    /// <summary>
    /// Plays the given audio clip in the position given
    /// Note: Wont play if audio clip is marked single instance and is already playing
    /// </summary>
    public void Play(Bank type, Vector3 pos)
    {
        var clip = m_AudioFiles[(int)type];

        // Don't play the audio clip if it's marked as a single instance 
        // and it's already playing
        if (clip.SingleInstance && clip.Running)
        {
            return;
        }

        var newGO = m_AudioSources.New(pos);
        var goCB = newGO.GetComponent<CustomBehaviour>();

        newGO.transform.position = pos;
        goCB.GetAudioSource.clip = clip.ClipFile[0];
        clip.Running = true;
        newGO.SetActive(true);

        Timing.RunCoroutine(StoreClip(goCB, clip), Segment.SlowUpdate);
    }

    public void PlayRandom(Bank type, Vector3 pos)
    {
        var clip = m_AudioFiles[(int)type];

        // Don't play the audio clip if it's marked as a single instance 
        // and it's already playing
        if (clip.SingleInstance && clip.Running)
        {
            return;
        }

        var newGO = m_AudioSources.New(pos);
        var goCB = newGO.GetComponent<CustomBehaviour>();
        int randomClip = UnityEngine.Random.Range(0, clip.ClipFile.Length);

        newGO.transform.position = pos;
        goCB.GetAudioSource.clip = clip.GetNextClip();
        clip.Running = true;
        newGO.SetActive(true);

        Timing.RunCoroutine(StoreClip(goCB, clip), Segment.SlowUpdate);
    }
}

