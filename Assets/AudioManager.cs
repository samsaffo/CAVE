using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;


public class AudioManager : MonoBehaviour
{

    [Header("Environmental Sound")]
    public Audios[] environmentalSounds;
    public AudioSource environmentalSource;

    [Header("SFX Sound")]
    public Audios[] SFXSounds;
    public AudioSource SFXSource;

    //Sounds which will play randomly in the background... can be anything from gooses to cars honking.
    [Header("Random Sound")]
    public Audios[] randomSounds;
    public AudioSource randomSource;

    [Header("SFX Simultaneously Sound")]
    public Audios[] SFXSimultaneouslySounds;
    public AudioSource SFXSimultaneouslySource;

    [Header("Truck Sound")]
    public Audios[] TruckSounds;

    [Header("Container Sound")]
    public Audios[] ContainerSounds;
    public AudioSource SpreaderSource;

    public static AudioManager Instance;
    
    private Queue<Audios> SFXaudioQueue = new Queue<Audios>();

    private string _currentAudioPlaying = "";
    public string currentAudioPlaying
    {
        get { return _currentAudioPlaying; }
    }
    public int random_sound_interval = 35; // seconds
    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public Audios GetSound(string name, Audios[] arr)
    {
        return Array.Find(arr, x => x.name == name);
    }

    private void Start()
    {
        PlayEnviromental("wind_noise"); // constant loop

        StartCoroutine(PlayEnvironmentalFluctuationSounds()); // starts random goose noises
    }

    public void CancelSFX()
    {
        SFXaudioQueue.Clear();
        SFXSource.Stop();
    }

    public bool isCurrentlyPlaying(AudioSource audioSource)
    {
        return audioSource != null && audioSource.isPlaying;
    }

    public delegate void PlaySoundDelegate(Audios audio);
    private IEnumerator PlayQueue(Queue<Audios> audioQueue, PlaySoundDelegate playsoundFunction)
    {
        while (audioQueue.Count > 0)
        {
            Audios clip = audioQueue.Dequeue();
            playsoundFunction(clip);

            yield return new WaitForSeconds(clip.clip.length);
        }
    }
    private void QueueAudio(Queue<Audios> audioQueue, AudioSource audioSource, PlaySoundDelegate playsoundFunction)
    {
        if (!audioSource.isPlaying)
        {
            StartCoroutine(PlayQueue(audioQueue, playsoundFunction));
        }
    }
    public void QueueSFXAudio(string[] names)
    {
        SFXaudioQueue.Clear();

        foreach (string name in names)
        {
            Audios s = GetSound(name, SFXSounds);
            SFXaudioQueue.Enqueue(s);
        }
        QueueAudio(SFXaudioQueue, SFXSource, PlaySFX);
    }

    public void QueueTruckAudio(string[] names, Queue<Audios> audioQueue, AudioSource audioSource)
    {
        audioQueue.Clear();

        foreach (string name in names)
        {
            Audios s = GetSound(name, TruckSounds);
            audioQueue.Enqueue(s);
        }

        QueueAudio(audioQueue, audioSource, (Audios audio) => PlaySound(audio, audioSource));
    }

    public void PlaySound(Audios audio, AudioSource audioSource)
    {
        audioSource.clip = audio.clip;
        audioSource.loop = audio.loop;
        audioSource.Play();
    }

    public void PlaySoundSimultaneously(Audios audio, AudioSource audioSource, float volumeScale = 1.0f)
    {
        audioSource.clip = audio.clip;
        audioSource.PlayOneShot(audioSource.clip, volumeScale);
    }


    public void PlayEnviromental(Audios audio) => PlaySound(audio, environmentalSource);
    public void PlayEnviromental(string name)
    {
        Audios s = GetSound(name, environmentalSounds);

        if (s == null)
        {
            Debug.Log($"Enviromental sound {name} not found!");
        }
        else
        {
            PlayEnviromental(s);
        }
    }

    public void PlaySFX(Audios audio)
    {
        PlaySound(audio, SFXSource);
        _currentAudioPlaying = name;
    }

    public void PlaySFX(string name)
    {
        Audios s = GetSound(name, SFXSounds);

        if (s == null)
        {
            Debug.Log($"SFX sound {name} not found!");
        }
        else
        {
            PlaySFX(s);
        }
    }

    public void PlaySFXSimultaneously(Audios audio, float volumeScale = 1.0f) => PlaySoundSimultaneously(audio, SFXSimultaneouslySource, volumeScale);
    public void PlaySFXSimultaneously(string name, float volumeScale = 1.0f)
    {
        Audios s = GetSound(name, SFXSimultaneouslySounds);

        if (s == null)
        {
            Debug.Log($"SFX simultaneously sound {name} not found!");
        }
        else
        {
            PlaySFXSimultaneously(s, volumeScale);
        }
    }

    IEnumerator PlayEnvironmentalFluctuationSounds()
    {
        while (true) //Keep loop going at all time
        {
            // only attempt to play a sound if sound is not  playing.
            if (randomSource != null && !randomSource.isPlaying)
            {
                AudioClip clip = randomSounds[UnityEngine.Random.Range(0, randomSounds.Length)].clip;
                randomSource.PlayOneShot(clip);
            }

            yield return new WaitForSeconds(random_sound_interval); 
        }
    }


}

