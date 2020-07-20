using System;
using UnityEngine;

public class SpectrumAnalyzer : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    public enum AudioOutputChannel { Stereo, Left, Right }

    [SerializeField, Tooltip("The AudioSource output channel to analyze.")]
    private AudioOutputChannel channel = AudioOutputChannel.Stereo;

    //Documentation: https://docs.unity3d.com/ScriptReference/FFTWindow.html
    [SerializeField]
    private FFTWindow FFTWindow = FFTWindow.Hamming;

    [SerializeField, 
     Tooltip("Used for the initial highest frequency band values when normalizing frequency band data to values between 0 and 1.")]
    private float audioProfile = 0f;

    private float[] sampleData = new float[512];

    public class SpectralAnalysisEventSampler : DynamicEventSampler<SpectralAnalysisData>
    {
        public SpectralAnalysisEventSampler(Func<EventSampleResult> eventSampler) : base(eventSampler) { }
    }

    #region 8 Band Analysis

    private float[] freqBands8 = new float[8];
    private float[] freqBand8Buffer = new float[8];
    private float[] freqBandBuffer8Decrease = new float[8];
    [SerializeField] private float[] freqBand8Highest = new float[8];
    private float[] audioBands8 = new float[8];
    private float[] audioBand8Buffer = new float[8];
    private float amplitude8 = 0f;
    private float amplitudeBuffer8 = 0f;
    private float amplitudeHighest8 = 0f;

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
    [SerializeField] private float[] freqBand64Highest = new float[64];
    private float[] audioBands64 = new float[64];
    private float[] audioBand64Buffer = new float[64];
    private float amplitude64 = 0f;
    private float amplitudeBuffer64 = 0f;
    private float amplitudeHighest64 = 0f;

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

    private void Start()
    {
        freqBand8Highest.setValues(audioProfile);
        freqBand64Highest.setValues(audioProfile);
    }

    private void getSampleData()
    {
        if (channel == AudioOutputChannel.Left)
            audioSource.GetSpectrumData(sampleData, 0, FFTWindow);
        else if (channel == AudioOutputChannel.Right)
            audioSource.GetSpectrumData(sampleData, 1, FFTWindow);
        else //Stereo
        {
            float[] leftSampleData = new float[512];
            float[] rightSampleData = new float[512];

            audioSource.GetSpectrumData(leftSampleData, 0, FFTWindow);
            audioSource.GetSpectrumData(rightSampleData, 1, FFTWindow);

            for (int i = 0; i < sampleData.Length; i++)
                sampleData[i] += (leftSampleData[i] + rightSampleData[i]) / 2;
        }
    }

    #region 8 Band Analysis

    private SpectralAnalysisEventSampler.EventSampleResult sample8BandAnalysis()
    {
        createFrequencyBands8();
        bufferFrequencyBands8();
        createRangedAudioBands8();
        getAmplitude8();

        //Only fire event if data does not contain float.NaN
        bool fireEvent = !audioBands8.contains(float.NaN) &&
            !audioBand8Buffer.contains(float.NaN) &&
            !amplitude8.Equals(float.NaN) &&
            !amplitudeBuffer8.Equals(float.NaN);

        //Only populate if event should be fired
        SpectralAnalysisData data = fireEvent ?
            new SpectralAnalysisData
            {
                audioBandsRaw = audioBands8,
                audioBandBuffer = audioBand8Buffer,
                amplitudeRaw = amplitude8,
                amplitudeBuffer = amplitudeBuffer8
            } :
            default;

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
            
            freqBands8[i] = average * 10;
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
            if (freqBands8[i] > freqBand8Highest[i]) //Track max value of frequency band
                freqBand8Highest[i] = freqBands8[i];

            //Set audio bands to value between 0 and 1
            audioBands8[i] = freqBands8[i] / freqBand8Highest[i];
            audioBand8Buffer[i] = freqBand8Buffer[i] / freqBand8Highest[i];
        }
    }

    private void getAmplitude8()
    {
        float amplitudeSum = 0f;
        float amplitudeBufferSum = 0f;

        for (int i = 0; i < audioBands8.Length; i++)
        {
            amplitudeSum += audioBands8[i];
            amplitudeBufferSum += audioBand8Buffer[i];
        }

        float amplitudeAvg = amplitudeSum / audioBands8.Length;
        float amplitudeBufferAvg = amplitudeBufferSum / audioBands8.Length;

        if (amplitudeAvg > amplitudeHighest8)
            amplitudeHighest8 = amplitudeAvg;

        amplitude8 = amplitudeAvg / amplitudeHighest8;
        amplitudeBuffer8 = amplitudeBufferAvg / amplitudeHighest8;
    }

    #endregion 8 Band Analysis

    #region 64 Band Analysis

    private SpectralAnalysisEventSampler.EventSampleResult sample64BandAnalysis()
    {
        createFrequencyBands64();
        bufferFrequencyBands64();
        createRangedAudioBands64();
        getAmplitude64();

        //Only fire event if data does not contain float.NaN
        bool fireEvent = !audioBands64.contains(float.NaN) &&
            !audioBand64Buffer.contains(float.NaN) &&
            !amplitude64.Equals(float.NaN) &&
            !amplitudeBuffer64.Equals(float.NaN);

        //Only populate if event should be fired
        SpectralAnalysisData data = fireEvent ?
            new SpectralAnalysisData
            {
                audioBandsRaw = audioBands64,
                audioBandBuffer = audioBand64Buffer,
                amplitudeRaw = amplitude8,
                amplitudeBuffer = amplitudeBuffer8
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

            freqBands64[i] = average * 80;
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
            if (freqBands64[i] > freqBand64Highest[i]) //Track max value of frequency band
                freqBand64Highest[i] = freqBands64[i];

            //Set audio bands to value between 0 and 1
            audioBands64[i] = freqBands64[i] / freqBand64Highest[i];
            audioBand64Buffer[i] = freqBand64Buffer[i] / freqBand64Highest[i];
        }
    }

    private void getAmplitude64()
    {
        float amplitudeSum = 0f;
        float amplitudeBufferSum = 0f;

        for (int i = 0; i < audioBands64.Length; i++)
        {
            amplitudeSum += audioBands64[i];
            amplitudeBufferSum += audioBand64Buffer[i];
        }

        float amplitudeAvg = amplitudeSum / audioBands64.Length;
        float amplitudeBufferAvg = amplitudeBufferSum / audioBands64.Length;

        if (amplitudeAvg > amplitudeHighest64)
            amplitudeHighest64 = amplitudeAvg;

        amplitude64 = amplitudeAvg / amplitudeHighest64;
        amplitudeBuffer64 = amplitudeBufferAvg / amplitudeHighest64;
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
    /// <summary>
    /// Values between 0 and 1 representing raw frequency bands.
    /// </summary>
    public float[] audioBandsRaw;

    /// <summary>
    /// Values between 0 and 1 representing smoothed frequency band values.
    /// </summary>
    public float[] audioBandBuffer;

    /// <summary>
    /// Average amplitude of the audio bands.
    /// </summary>
    public float amplitudeRaw;

    /// <summary>
    /// Average amplitude of the audio band buffer.
    /// </summary>
    public float amplitudeBuffer;
}