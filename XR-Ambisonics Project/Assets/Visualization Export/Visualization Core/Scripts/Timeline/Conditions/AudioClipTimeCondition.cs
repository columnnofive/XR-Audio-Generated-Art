using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipTimeCondition : TimelineCondition
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField, Tooltip("Playback time the audio clip must reach to satisfy the condition.")]
    private float targetClipTime = 0f;

    [SerializeField, ReadOnlyField]
    private float currentClipTime = 0f;

    public override bool isMet()
    {
        currentClipTime = audioSource.time;
        return audioSource.time >= targetClipTime;
    }
}
