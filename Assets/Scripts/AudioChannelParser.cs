using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioChannelParser : MonoBehaviour
{
    [SerializeField]
    private AudioClip multiChannelClip;

    private float[] multiChannelData;

    [SerializeField]
    private int channel;

    private int readPosition = 0;

    private AudioClip channelClip;
    private AudioSource audioSource;

    private int channels;
    private int samples;
    private int frequency;

    private void OnValidate()
    {
        if (multiChannelClip)
        {
            //Constrain channel to a value between 0 and the number of channels in the clip
            int clipChannels = multiChannelClip.channels;
            if (channel > clipChannels - 1)
                channel = clipChannels - 1;
            else if (channel < 0)
                channel = 0;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        getMultiChannelData();
        createChannelAudioClip();
    }

    private void getMultiChannelData()
    {
        channels = multiChannelClip.channels;
        samples = multiChannelClip.samples;
        frequency = multiChannelClip.frequency;

        multiChannelData = new float[samples * channels];
        multiChannelClip.GetData(multiChannelData, readPosition);
    }

    private void createChannelAudioClip()
    {
        string clipName = multiChannelClip.name + " - " + channel;
        channelClip = AudioClip.Create(clipName, samples, 1, frequency, true, onAudioRead, onAudioSetPosition);

        audioSource.clip = channelClip;
        audioSource.Play();
    }

    private void onAudioRead(float[] data)
    {
        for (int j = readPosition * channels, i = 0;
            j < multiChannelData.Length && i < data.Length;
            j += channels, i++, readPosition++)
        {
            data[i] = multiChannelData[j + channel];
        }
    }

    private void onAudioSetPosition(int newPosition)
    {
        readPosition = newPosition;
    }
}
