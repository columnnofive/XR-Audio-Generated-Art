﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AudioVisualizationController
{
    public SpectrumAnalyzer analyzer;    
    public AnalysisMode analysisMode;    
    public AudioBandDataType audioBandDataType;    
    public AmplitudeDataType amplitudeDataType;

    [Header("Data Filtering")]

    [Tooltip("Filters to apply to the visualization data. Applied in numerical order.")]
    public List<DataFilter> dataFilters;

    /// <summary>
    /// Number of audio bands contained in the analysis data.
    /// </summary>
    public int bands { get; private set; }

    private UnityAction<VisualizationData> visualizationListener;

    private bool enabled = false;

    private bool canEnable
    {
        get
        {
            return !enabled &&
                analyzer &&
                visualizationListener != null;
        }
    }

    private bool canDisable
    {
        get
        {
            return enabled;
        }
    }

    public void setVisualizationListener(UnityAction<VisualizationData> visualizationListener)
    {
        this.visualizationListener = visualizationListener;
    }

    public void enable()
    {
        if (canEnable)
        {
            if (analysisMode == AnalysisMode.AudioBands8)
            {
                bands = 8;
                analyzer.OnAnalyzeBands8.addListener(handleSpectralAnalysis);
            }
            else // AnalysisMode.AudioBands64
            {
                bands = 64;
                analyzer.OnAnalyzeBands64.addListener(handleSpectralAnalysis);
            }

            enabled = true;
        }
    }

    public void disable()
    {
        if (canDisable)
        {
            visualizationListener = null;

            if (analysisMode == AnalysisMode.AudioBands8)
            {
                analyzer.OnAnalyzeBands8.removeListener(handleSpectralAnalysis);
            }
            else // AnalysisMode.AudioBands64
            {
                analyzer.OnAnalyzeBands64.removeListener(handleSpectralAnalysis);
            }

            enabled = false;
        }
    }

    private void handleSpectralAnalysis(SpectralAnalysisData data)
    {
        float[] audioBands = null;

        if (audioBandDataType == AudioBandDataType.AudioBandRaw)
            audioBands = data.audioBandsRaw;
        else // AudioBandDataType.AudioBandBuffer
            audioBands = data.audioBandBuffer;

        float amplitude = 0f;

        if (amplitudeDataType == AmplitudeDataType.AmplitudeRaw)
            amplitude = data.amplitudeRaw;
        else // AmplitudeDataType.AmplitudeBuffer
            amplitude = data.amplitudeBuffer;

        VisualizationData visualizationData = new VisualizationData
        {
            audioBands = audioBands,
            amplitude = amplitude
        };

        filterData(ref visualizationData); //Apply any set data filters
        visualizationListener?.Invoke(visualizationData); //Notify listener
    }

    private void filterData(ref VisualizationData data)
    {
        foreach (DataFilter filter in dataFilters)
        {
            if (filter) //Filter not null
                data = filter.filter(data);
        }
    }

    public static int getBandCount(AnalysisMode analysisMode)
    {
        return analysisMode == AnalysisMode.AudioBands8 ? 8 : 64;
    }
}

public enum AnalysisMode
{
    AudioBands8,
    AudioBands64
}

public enum AudioBandDataType
{
    AudioBandRaw,
    AudioBandBuffer
}

public enum AmplitudeDataType
{
    AmplitudeRaw,
    AmplitudeBuffer
}

public struct VisualizationData
{
    private const float dataMin = 0f;
    private const float dataMax = 1f;

    private float[] _audioBands;
    public float[] audioBands
    {
        get
        {
            return _audioBands;
        }
        set
        {
            if (value == null)
                _audioBands = value;
            else
            {
                _audioBands = value;

                //Clamp to min and max values
                for (int i = 0; i < _audioBands.Length; i++)
                    _audioBands[i] = Mathf.Clamp(_audioBands[i], dataMin, dataMax);
            }
        }
    }

    private float _amplitude;
    public float amplitude
    {
        get
        {
            return _amplitude;
        }
        set
        {
            //Clamp to min and max values
            _amplitude = Mathf.Clamp(value, dataMin, dataMax);
        }
    }
}