using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceSynchronizer : MonoBehaviour
{
    private AudioSource masterAudioSource;

    [SerializeField]
    private AudioSource[] synchronizedAudioSources;

    private void Start()
    {
        masterAudioSource = GetComponent<AudioSource>();
    }

    private void updateControllerAudioSources()
    {
        if (masterAudioSource.clip)
        {
            foreach (AudioSource audioSource in synchronizedAudioSources)
            {
                if (audioSource.time != masterAudioSource.time)
                    audioSource.time = masterAudioSource.time;
            }
        }
    }
    
    private void Update()
    {
        updateControllerAudioSources();
    }
}
