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
}
