using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTimeController : MonoBehaviour
{
    private AudioSource audioSource;

    [Range(0, 1), SerializeField]
    private float clipTimeProportion = 0f;
    
    private float previousClipTimeProportion = 0f;

    [ReadOnlyField, SerializeField]
    private float currentClipTime = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void updateClipTime()
    {
        if (audioSource.clip)
        {
            if (clipTimeProportion != previousClipTimeProportion) //Clip time was set manually
                audioSource.time = (audioSource.clip.length - 1) * clipTimeProportion;
            else //set clipTime to follow audioSource.time
                clipTimeProportion = audioSource.time / (audioSource.clip.length - 1);

            currentClipTime = audioSource.time; //Debug purposes only

            previousClipTimeProportion = clipTimeProportion;
        }
    }
    
    private void Update()
    {
        updateClipTime();
    }
}
