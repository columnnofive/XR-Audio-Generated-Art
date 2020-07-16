using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMasterController : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] audioSources;

    [Range(0, 1), SerializeField]
    private float clipTime = 0f;
    
    private float previousClipTime = 0f;

    private void updateClipTime()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (clipTime != previousClipTime) //Clip time was set manually
                audioSource.time = audioSource.clip.length * clipTime;
            else //set clipTime to follow audioSource.time
                clipTime = audioSource.time / (audioSource.clip.length - 1);
        }

        previousClipTime = clipTime;
    }
    
    private void Update()
    {
        updateClipTime();
    }
}
