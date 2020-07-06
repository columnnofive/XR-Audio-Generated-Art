using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioMasterController : MonoBehaviour
{
    private AudioSource audioSource;

    [Range(0, 1), SerializeField]
    private float clipTime = 0f;
    
    private float previousClipTime = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void updateClipTime()
    {
        if (clipTime != previousClipTime) //Clip time was set manually
            audioSource.time = audioSource.clip.length * clipTime;
        else //set clipTime to follow audioSource.time
            clipTime = audioSource.time / (audioSource.clip.length - 1);

        previousClipTime = clipTime;
    }
    
    private void Update()
    {
        updateClipTime();
    }
}
