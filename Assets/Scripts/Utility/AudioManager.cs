using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;

public enum ESoundChannel
{
    MUSIC,
    SFX,
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] public float spatialBlend = 0.75f;
    [SerializeField] private int initialAudioSources = 10;
    #pragma warning disable 0649
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] private AudioClip[] audioClips;
    #pragma warning restore 0649

    public static AudioManager Instance;

    private Dictionary<string, SoundClip> sounds;
    private List<AudioSource> audioSources;
    private int audioSourceID;

    private readonly bool DEBUGGING = false;
    
    void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        sounds = new Dictionary<string, SoundClip>();
        audioSources = new List<AudioSource>();
        audioSourceID = 0;

        foreach (AudioClip ac in audioClips)
        {
            if (DEBUGGING) Debug.Log($"New SoundClip {ac.name} added.");
            sounds.Add(ac.name, new SoundClip(ac));
        }
        
        for (int i = 0; i < initialAudioSources; i++)
        {
            CreateNewAudioSource();
        }
    }

    /// <summary>
    /// Plays a sound at the location the AudioManager is at.
    /// </summary>
    /// <param name="soundName"> Name of SoundClip played </param>
    /// <param name="loop"> Whether the SoundClip loops </param>
    /// <param name="onFinish"> Function called when the SoundClip is done playing </param>
    /// <returns> The played SoundClip </returns>
    public SoundClip PlaySound(string soundName, ESoundChannel soundChannel, bool loop = false, UnityAction onFinish = null)
    {
        if (DEBUGGING) Debug.Log($"Playing Sound {soundName}");

        SoundClip soundClip = new SoundClip(sounds[soundName]);
        soundClip.soundChannel = soundChannel;
        soundClip.loop = loop;

        if (onFinish != null) { soundClip.onFinish += onFinish; }

        AudioSource audioSource = soundClip.AttachToAudioSource(NextAudiosource());
        audioSource.spatialBlend = 0;
        soundClip.audioSource.Play();

        if (!loop) { StartCoroutine(ExecuteAfterSeconds(soundClip.onFinish, soundClip.Length)); }

        return soundClip;
    }

    /// <summary>
    /// Plays a sound at a specified location.
    /// </summary>
    /// <param name="soundName"> Name of SoundClip played </param>
    /// <param name="location"> Location SoundClip played at </param>
    /// <param name="loop"> Whether the SoundClip loops </param>
    /// <param name="onFinish"> Function called when the SoundClip is done playing </param>
    /// <returns> The played SoundClip </returns>
    public SoundClip PlaySoundAtLocation(string soundName, ESoundChannel soundChannel, Vector3 location, bool loop = false, UnityAction onFinish = null)
    {
        if (DEBUGGING) Debug.Log($"Playing Sound {soundName} at location {location}");

        SoundClip soundClip = new SoundClip(sounds[soundName]);
        soundClip.soundChannel = soundChannel;
        soundClip.loop = loop;

        if (onFinish != null) { soundClip.onFinish += onFinish; }

        AudioSource audioSource = soundClip.AttachToAudioSource(NextAudiosource());
        audioSource.spatialBlend = spatialBlend;
        audioSource.transform.position = location;
        soundClip.audioSource.Play();

        if (!loop) { StartCoroutine(ExecuteAfterSeconds(soundClip.onFinish, soundClip.Length)); }

        return soundClip;
    }

    /// <summary>
    /// Plays a sound attached to a GameObject.
    /// </summary>
    /// <param name="soundName"> Name of SoundClip played </param>
    /// <param name="parent"> GameObject that will parent the sound </param>
    /// <param name="loop"> Whether the SoundClip loops </param>
    /// <param name="onFinish"> Function called when the SoundClip is done playing </param>
    /// <returns> The played SoundClip </returns>
    public SoundClip PlaySoundWithParent(string soundName, ESoundChannel soundChannel, GameObject parent, bool loop = false, UnityAction onFinish = null)
    {
        if (DEBUGGING) Debug.Log($"Playing Sound {soundName} attached to GameObject {parent}");

        SoundClip soundClip = new SoundClip(sounds[soundName]);
        soundClip.soundChannel = soundChannel;
        soundClip.loop = loop;

        if (onFinish != null) { soundClip.onFinish += onFinish; }

        AudioSource audioSource = soundClip.AttachToAudioSource(NextAudiosource());
        audioSource.spatialBlend = spatialBlend;
        audioSource.transform.parent = parent.transform;
        soundClip.audioSource.Play();

        if (!loop) { StartCoroutine(ExecuteAfterSeconds(soundClip.onFinish, soundClip.Length)); }

        return soundClip;
    }

    /// <summary>
    /// Returns the next AudioSource in the List audioSources not being used. If there isn't one it creates one and adds it to the List.
    /// </summary>
    /// <returns> An inactive AudioSource </returns>
    private AudioSource NextAudiosource()
    {
        AudioSource audioSource = audioSources[0];

        int iter = 0;
        while(audioSource.gameObject.activeSelf)
        {
            if(++iter >= audioSources.Count)
            {
                return CreateNewAudioSource();
            }
            audioSource = audioSources[iter];
        }

        return audioSource;
    }

    /// <summary>
    /// Creates a new AudioSource and addes it to audioSources.
    /// </summary>
    /// <param name="active"> Whether the new AudioSource is active </param>
    /// <returns> The Created AudioSource </returns>
    private AudioSource CreateNewAudioSource(bool active = false)
    {
        GameObject go = new GameObject($"AudioSource ({audioSourceID++})");
        go.transform.parent = transform;
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        audioSource.spatialBlend = 1f;

        audioSources.Add(audioSource);
        go.SetActive(active);

        return audioSource;
    }

    private static IEnumerator ExecuteAfterSeconds(UnityAction action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}

public class SoundClip
{
    public bool loop;
    public UnityAction onFinish;
    public AudioSource audioSource;
    public ESoundChannel soundChannel;

    private readonly AudioClip audioClip;

    public float Progress
    {
        get
        {
            if (audioSource == null || audioClip == null)
            {
                return 0.0f;
            }
            return (float)audioSource.timeSamples / (float)audioClip.samples;
        }
    }
    public bool isPlaying
    {
        get
        {
            return audioSource != null && audioSource.isPlaying;
        }
    }
    public bool IsFinished
    {
        get
        {
            return !loop && Progress >= 1;
        }
    }
    public float Length
    {
        get { return audioClip.length; }
    }

    public SoundClip(AudioClip audioClip)
    {
        this.audioClip = audioClip;
        onFinish += DeactivateAudioSource;
    }

    public SoundClip(SoundClip soundClip) 
        : this(soundClip.audioClip)
    { }

    /// <summary>
    /// Sets audioSource to the provided AudioSource and sets that audioSource's clip to audioClip.
    /// </summary>
    /// <param name="audioSource"></param>
    /// <returns> the provided AudioSource </returns>
    public AudioSource AttachToAudioSource(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        audioSource.clip = audioClip;
        audioSource.transform.position = Vector3.zero;
        audioSource.transform.parent = null;
        audioSource.gameObject.SetActive(true);
        
        switch(soundChannel)
        {
            case ESoundChannel.MUSIC:
                audioSource.outputAudioMixerGroup = AudioManager.Instance.audioMixer.FindMatchingGroups("Music")[0];
                break;
            case ESoundChannel.SFX:
                audioSource.outputAudioMixerGroup = AudioManager.Instance.audioMixer.FindMatchingGroups("SFX")[0];
                break;
        }

        return audioSource;
    }

    /// <summary>
    /// Set audioSource inactive. Clears onFinish and adds this function back.
    /// </summary>
    private void DeactivateAudioSource()
    {
        audioSource.gameObject.SetActive(false);
        audioSource.transform.parent = AudioManager.Instance.transform;
        onFinish = DeactivateAudioSource;
    }
}