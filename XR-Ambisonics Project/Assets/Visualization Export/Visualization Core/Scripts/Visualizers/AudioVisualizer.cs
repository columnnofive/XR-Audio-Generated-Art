using System.Collections.Generic;
using UnityEngine;

public abstract class AudioVisualizer : AudioVisualizerBase
{
    [SerializeField]
    private AudioVisualizationController visualizationController;

    public AudioVisualizationController VisualizationController
    {
        get
        {
            return visualizationController;
        }
    }

    protected virtual void OnValidate() { }

    protected virtual void OnEnable()
    {
        visualizationController.setVisualizationListener(visualizeData);
        visualizationController.enable();
    }

    protected virtual void OnDisable()
    {
        visualizationController.disable();
    }

    protected abstract void visualizeData(VisualizationData data);

    #region Editor Accessible Setters

    public void setAudioBandDataType(AudioBandDataType dataType)
    {
        VisualizationController.audioBandDataType = dataType;
    }

    public void setAmplitudeDataType(AmplitudeDataType dataType)
    {
        VisualizationController.amplitudeDataType = dataType;
    }

    public void addDataFilter(DataFilter filter)
    {
        VisualizationController.dataFilters.Add(filter);
    }

    public void removeDataFilter(DataFilter filter)
    {
        VisualizationController.dataFilters.Remove(filter);
    }

    #endregion Editor Accessible Setters
}