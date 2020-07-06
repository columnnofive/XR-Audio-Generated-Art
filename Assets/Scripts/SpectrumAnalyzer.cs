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

    public class SpectralAnalysisEventSampler : DynamicEventSampler<SpectralAnalysisData>
    {
        public SpectralAnalysisEventSampler(Func<EventSampleResult> eventSampler) : base(eventSampler) { }
    }

    #region 8 Band Analysis

    private float[] freqBands8 = new float[8];
    private float[] freqBand8Buffer = new float[8];
    private float[] freqBandBuffer8Decrease = new float[8];
    private float[] freqBand8Max = new float[8];
    private float[] audioBands8 = new float[8];
    private float[] audioBand8Buffer = new float[8];

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

    #endregion 8 Band Analysis

    #region 64 Band Analysis

    private float[] freqBands64 = new float[64];
    private float[] freqBand64Buffer = new float[64];
    private float[] freqBandBuffer64Decrease = new float[64];
    private float[] freqBand64Max = new float[64];
    private float[] audioBands64 = new float[64];
    private float[] audioBand64Buffer = new float[64];

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

    #endregion 64 Band Analysis

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
        createRangedAudioBands8();

        //Only fire event if data does not contain float.NaN
        bool fireEvent = !arrayContains(audioBands8, float.NaN) &&
            !arrayContains(audioBand8Buffer, float.NaN);

        //Only populate if event should be fired
        SpectralAnalysisData data = fireEvent ?
            new SpectralAnalysisData
            {
                audioBands = audioBands8,
                audioBandBuffer = audioBand8Buffer
            } :
            default;

        //Only fire event if data does not contain float.NaN
        //bool fireEvent = !arrayContains(freqBands8, float.NaN) &&
        //    !arrayContains(freqBand8Buffer, float.NaN);

        ////Only populate if event should be fired
        //SpectralAnalysisData data = fireEvent ?
        //    new SpectralAnalysisData
        //    {
        //        audioBands = freqBands8,
        //        audioBandBuffer = freqBand8Buffer
        //    } :
        //    default;

        return new SpectralAnalysisEventSampler.EventSampleResult
        {
            eventOccurred = fireEvent,
            eventParam = data
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
            
            freqBands8[i] = average;
        }
    }

    private void bufferFrequencyBands8()
    {
        for (int i = 0; i < freqBand8Buffer.Length; i++)
        {
            if (freqBands8[i] > freqBand8Buffer[i])
            {
                freqBand8Buffer[i] = freqBands8[i];
                freqBandBuffer8Decrease[i] = 0.005f;
            }

            if (freqBands8[i] < freqBand8Buffer[i])
            {
                freqBandBuffer8Decrease[i] = (freqBand8Buffer[i] - freqBands8[i]) / freqBand8Buffer.Length;
                freqBand8Buffer[i] -= freqBandBuffer8Decrease[i];
            }
        }
    }

    private void createRangedAudioBands8()
    {
        for (int i = 0; i < freqBands8.Length; i++)
        {
            if (freqBands8[i] > freqBand8Max[i]) //Track max value of frequency band
                freqBand8Max[i] = freqBands8[i];

            //Set audio bands to value between 0 and 1
            audioBands8[i] = freqBands8[i] / freqBand8Max[i];
            audioBand8Buffer[i] = freqBand8Buffer[i] / freqBand8Max[i];
        }
    }

    #endregion 8 Band Analysis

    #region 64 Band Analysis

    private SpectralAnalysisEventSampler.EventSampleResult sample64BandAnalysis()
    {
        createFrequencyBands64();
        bufferFrequencyBands64();
        createRangedAudioBands64();

        //Only fire event if data does not contain float.NaN
        bool fireEvent = !arrayContains(audioBands64, float.NaN) &&
            !arrayContains(audioBand64Buffer, float.NaN);

        //Only populate if event should be fired
        SpectralAnalysisData data = fireEvent ?
            new SpectralAnalysisData
            {
                audioBands = audioBands64,
                audioBandBuffer = audioBand64Buffer
            } :
            default;

        return new SpectralAnalysisEventSampler.EventSampleResult
        {
            eventOccurred = fireEvent,
            eventParam = data
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

            freqBands64[i] = average;
        }
    }

    private void bufferFrequencyBands64()
    {
        for (int i = 0; i < freqBand64Buffer.Length; i++)
        {
            if (freqBands64[i] > freqBand64Buffer[i])
            {
                freqBand64Buffer[i] = freqBands64[i];
                freqBandBuffer64Decrease[i] = 0.005f;
            }

            if (freqBands64[i] < freqBand64Buffer[i])
            {
                freqBandBuffer64Decrease[i] = (freqBand64Buffer[i] - freqBands64[i]) / freqBand64Buffer.Length;
                freqBand64Buffer[i] -= freqBandBuffer64Decrease[i];
            }
        }
    }

    private void createRangedAudioBands64()
    {
        for (int i = 0; i < freqBands64.Length; i++)
        {
            if (freqBands64[i] > freqBand64Max[i]) //Track max value of frequency band
                freqBand64Max[i] = freqBands64[i];

            //Set audio bands to value between 0 and 1
            audioBands64[i] = freqBands64[i] / freqBand64Max[i];
            audioBand64Buffer[i] = freqBand64Buffer[i] / freqBand64Max[i];
        }
    }

    #endregion 64 Band Analysis

    private static bool arrayContains(float[] array, float target)
    {
        return Array.Find(array, val => val.Equals(target)) != default;
    }

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
    /// <summary>
    /// Values between 0 and 1 representing raw frequency bands.
    /// </summary>
    public float[] audioBands;

    /// <summary>
    /// Values between 0 and 1 representing smoothed frequency band values.
    /// </summary>
    public float[] audioBandBuffer;
}