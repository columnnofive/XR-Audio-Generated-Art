using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumAnalyzer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    /// <summary>
    /// 0 - Stereo
    /// 1 - Left
    /// 2 - Right
    /// </summary>
    [SerializeField]
    private int channel = 0; //0 is average?? other channels specify ambisonic channel, wxyz??

    //Documentation: https://docs.unity3d.com/ScriptReference/FFTWindow.html
    [SerializeField]
    private FFTWindow FFTWindow = FFTWindow.Hamming;
    
    private float[] sampleData = new float[512];

    private float[] freqBands8 = new float[8];
    private float[] freqBandBuffer8 = new float[8];
    private float[] freqBandBuffer8Decrease = new float[8];

    private float[] freqBands64 = new float[64];
    private float[] freqBandBuffer64 = new float[64];
    private float[] freqBandBuffer64Decrease = new float[64];

    public class SpectralAnalysisEventSampler : DynamicEventSampler<SpectralAnalysisData>
    {
        public SpectralAnalysisEventSampler(Func<EventSampleResult> eventSampler) : base(eventSampler) { }
    }

    private SpectralAnalysisEventSampler analysisBandsSampler8;
    public SpectralAnalysisEventSampler OnAnalyzeBands8
    {
        get
        {
            if (analysisBandsSampler8 == null)
                analysisBandsSampler8 = new SpectralAnalysisEventSampler(sample8BandAnalysis);
            return analysisBandsSampler8;
        }
    }

    private SpectralAnalysisEventSampler analysisBandsSampler64;
    public SpectralAnalysisEventSampler OnAnalyzeBands64
    {
        get
        {
            if (analysisBandsSampler64 == null)
                analysisBandsSampler64 = new SpectralAnalysisEventSampler(sample64BandAnalysis);
            return analysisBandsSampler64;
        }
    }

    private void OnValidate()
    {        
        if (audioSource.clip)
        {
            //Constrain channel to a value between 0 and the number of channels in the clip
            int clipChannels = audioSource.clip.channels;
            if (channel > clipChannels - 1)
                channel = clipChannels - 1;
            else if (channel < 0)
                channel = 0;
        }
    }

    private void Start()
    {
        Debug.Log("Clip channels: " + audioSource.clip.channels);
    }

    private void getSampleData()
    {
        audioSource.GetSpectrumData(sampleData, channel, FFTWindow);
    }

    #region 8 Band Analysis

    private SpectralAnalysisEventSampler.EventSampleResult sample8BandAnalysis()
    {
        createFrequencyBands8();
        bufferFrequencyBands8();

        return new SpectralAnalysisEventSampler.EventSampleResult
        {
            eventOccurred = true,
            eventParam = new SpectralAnalysisData
            {
                audioBands = freqBandBuffer8
            }
        };
    }    

    private void createFrequencyBands8()
    {
        int count = 0;
        for (int i = 0; i < freqBands8.Length; i++)
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
            
            freqBands8[i] = average * 10;
        }
    }

    private void bufferFrequencyBands8()
    {
        for (int i = 0; i < freqBandBuffer8.Length; i++)
        {
            if (freqBands8[i] > freqBandBuffer8[i])
            {
                freqBandBuffer8[i] = freqBands8[i];
                freqBandBuffer8Decrease[i] = 0.005f;
            }

            if (freqBands8[i] < freqBandBuffer8[i])
            {
                freqBandBuffer8[i] -= freqBandBuffer8Decrease[i];
                freqBandBuffer8Decrease[i] *= 1.2f;
            }
        }
    }

    #endregion 8 Band Analysis

    #region 64 Band Analysis

    private SpectralAnalysisEventSampler.EventSampleResult sample64BandAnalysis()
    {
        createFrequencyBands64();
        bufferFrequencyBands64();

        return new SpectralAnalysisEventSampler.EventSampleResult
        {
            eventOccurred = true,
            eventParam = new SpectralAnalysisData
            {
                audioBands = freqBandBuffer64
            }
        };
    }

    private void createFrequencyBands64()
    {
        int count = 0;
        int sampleCount = 1;
        int power = 0;

        for (int i = 0; i < freqBands64.Length; i++)
        {
            float average = 0;
            if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
            {
                power++;
                sampleCount = (int)Mathf.Pow(2, power);

                if (power == 3)
                    sampleCount -= 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += sampleData[count] * (count + 1);
                count++;
            }

            average /= count;

            freqBands64[i] = average * 80;
        }
    }

    private void bufferFrequencyBands64()
    {
        for (int i = 0; i < freqBandBuffer64.Length; i++)
        {
            if (freqBands64[i] > freqBandBuffer64[i])
            {
                freqBandBuffer64[i] = freqBands64[i];
                freqBandBuffer64Decrease[i] = 0.005f;
            }

            if (freqBands64[i] < freqBandBuffer64[i])
            {
                freqBandBuffer64[i] -= freqBandBuffer64Decrease[i];
                freqBandBuffer64Decrease[i] *= 1.2f;
            }
        }
    }

    #endregion 64 Band Analysis

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            getSampleData();

            OnAnalyzeBands8.update();
            OnAnalyzeBands64.update();
        }
    }
}

public struct SpectralAnalysisData
{
    public float[] audioBands;
}