using System.Collections.Generic;
using UnityEngine;

public abstract class AudioVisualizer : MonoBehaviour
{
    [SerializeField]
    private SpectrumAnalyzer analyzer;

    public enum AnalysisMode
    {
        AudioBands8,
        AudioBands64
    }

    [SerializeField]
    private AnalysisMode analysisMode;

    private Dictionary<AnalysisMode, int> analysisModeBands = new Dictionary<AnalysisMode, int>
    {
        { AnalysisMode.AudioBands8, 8 },
        { AnalysisMode.AudioBands64, 64 },
    };

    /// <summary>
    /// Number of audio bands contained in the analysis data.
    /// </summary>
    protected int bands { get; private set; }

    public enum AudioBandDataType
    {
        AudioBandsRaw,
        AudioBandBuffer
    }

    [SerializeField]
    private AudioBandDataType audioBandDataType;

    public enum AmplitudeDataType
    {
        AmplitudeRaw,
        AmplitudeBuffer
    }

    [SerializeField]
    private AmplitudeDataType amplitudeDataType;

    protected struct VisualizationData
    {
        public float[] audioBands;
        public float amplitude;
    }

    protected virtual void Start()
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
    }

    private void handleSpectralAnalysis(SpectralAnalysisData data)
    {
        float[] audioBands = null;

        if (audioBandDataType == AudioBandDataType.AudioBandsRaw)
            audioBands = data.audioBandsRaw;
        else // AudioBandDataType.AudioBandBuffer
            audioBands = data.audioBandBuffer;

        float amplitude = 0f;

        if (amplitudeDataType == AmplitudeDataType.AmplitudeRaw)
            amplitude = data.amplitudeRaw;
        else // AmplitudeDataType.AmplitudeBuffer
            amplitude = data.amplitudeBuffer;

        handleVisualizationData(new VisualizationData
        {
            audioBands = audioBands,
            amplitude = amplitude
        });
    }

    protected abstract void handleVisualizationData(VisualizationData data);
}
