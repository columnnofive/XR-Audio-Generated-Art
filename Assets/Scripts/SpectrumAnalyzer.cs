using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumAnalyzer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [Header("Spectrum Analysis")]

    [SerializeField]
    private int samples = 512; //Must be a power of 2

    /// <summary>
    /// 0 - Stereo
    /// 1 - Left
    /// 2 - Right
    /// </summary>
    [SerializeField]
    private int spectrumChannel = 0; //0 is average?? other channels specify ambisonic channel, wxyz??

    //Documentation: https://docs.unity3d.com/ScriptReference/FFTWindow.html
    [SerializeField]
    private FFTWindow FFTWindow = FFTWindow.Hamming;

    [Header("Frequency Bands")]

    [SerializeField]
    private int frequencyBands = 8; //Must be a power of 2

    public int FrequencyBandCount
    {
        get
        {
            return frequencyBands;
        }
    }

    private float[] sampleData;
    public float[] freqBands;

    private float[] freqBandBuffer;
    private float[] freqBandBufferDecrease;

    public float[] FrequencyBands
    {
        get
        {
            return freqBands;
        }
    }

    public bool old = true;

    private void OnValidate()
    {
        // Samples should be power 2 also between 64 and 8192??

        // Frequency bands should be power 2

        // Validate against audioSource.audioClip.channels
    }

    private void Start()
    {
        sampleData = new float[samples];

        freqBands = new float[frequencyBands];
        freqBandBuffer = new float[frequencyBands];
        freqBandBufferDecrease = new float[frequencyBands];

        Debug.Log("Clip channels: " + audioSource.clip.channels);
    }

    private void getSampleData()
    {
        audioSource.GetSpectrumData(sampleData, spectrumChannel, FFTWindow);
    }

    private void setFrequencyBands()
    {
        if (old)
        {
            int count = 0;
            for (int i = 0; i < frequencyBands; i++)
            {
                float average = 0;
                int sampleCount = (int)Mathf.Pow(2, i) * 2;

                if (i == 7) //Sum of frequencies is 510, get to 512
                    sampleCount += 2;

                for (int j = 0; j < sampleCount; j++)
                {
                    average += sampleData[count] * (count + 1);
                    count++;
                }

                average /= count;

                //frequencyBands[i] = average * 10;
                freqBands[i] = average * 1.25f * frequencyBands;
            }
        }
        else
        {
            int band = 0;
            for (int i = 0; i < freqBands.Length; i++)
            {
                float average = 0;

                /* As you increment on Frequency bands to set, get number of samples
                 * looking to get average of next based on for loop progress percentage */
                //int sampleCount = (int)Mathf.Lerp(2f, sampleData.Length - 1, i / ((float)freqBands.Length - 1));
                int sampleCount = (int)Mathf.Lerp(2f, sampleData.Length / 2, i / ((float)freqBands.Length - 1));

                /* always start the j index at the current value of band here
                 * if you always start from 0, band++ will increment out of _samples bounds */
                for (int j = band; j < sampleCount; j++)
                {
                    average += sampleData[band] * (band + 1);
                    band++;
                }

                average /= sampleCount;

                //freqBands[i] = average;
                freqBands[i] = average * 1.25f * frequencyBands;
            }
        }
    }
    
    private void Update()
    {
        if (audioSource.isPlaying)
        {
            getSampleData();
            setFrequencyBands();
        }
    }
}
