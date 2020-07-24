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

    #endregion Editor Accessible Setters
}

[System.Serializable]
public struct VisualizationAmplitudeTrigger
{
    [Range(0, 1), Tooltip("Amplitude that must be reached for the trigger.")]
    public float targetAmplitude;

    [Tooltip("Number of times the target amplitude must be reached for the trigger to be active.")]
    public float requiredRepetitions;

    [HideInInspector]
    public float repetitions;

    public bool checkTrigger(float amplitude)
    {
        if (amplitude < targetAmplitude)
            return false;

        //targetAmplitude reached
        repetitions++;
        
        //Triggered
        if (repetitions == requiredRepetitions)
        {
            repetitions = 0;
            return true;
        }
        else //Not triggered
            return false;
    }
}