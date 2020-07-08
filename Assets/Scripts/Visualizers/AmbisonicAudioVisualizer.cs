using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AmbisonicAudioVisualizer : AudioVisualizerBase
{
    [SerializeField]
    private AudioVisualizationController wController;

    private VisualizationData wData { get; set; }

    [SerializeField]
    private AudioVisualizationController xController;

    private VisualizationData xData { get; set; }

    [SerializeField]
    private AudioVisualizationController yController;

    private VisualizationData yData { get; set; }

    [SerializeField]
    private AudioVisualizationController zController;

    private VisualizationData zData { get; set; }

    [SerializeField]
    private bool autoEnable = true;

    /// <summary>
    /// Determines if the visualization data has been updated since last frame.
    /// </summary>
    private bool dataUpdated = false;

    private void OnEnable()
    {
        wController.setVisualizationListener(handleVisualizationDataW);
        xController.setVisualizationListener(handleVisualizationDataX);
        yController.setVisualizationListener(handleVisualizationDataY);
        zController.setVisualizationListener(handleVisualizationDataZ);

        if (autoEnable)
        {
            wController.enable();
            xController.enable();
            yController.enable();
            zController.enable();
        }
    }

    private void LateUpdate()
    {
        if (dataUpdated)
        {
            dataUpdated = false;
            visualizeAmbisonicData(wData, xData, yData, zData);
        }
    }

    private void OnDisable()
    {
        wController.disable();
        xController.disable();
        yController.disable();
        zController.disable();
    }

    private void handleVisualizationDataW(VisualizationData data)
    {
        wData = data;
        dataUpdated = true;
        visualizeDataW(data);
    }

    private void handleVisualizationDataX(VisualizationData data)
    {
        xData = data;
        dataUpdated = true;
        visualizeDataX(data);
    }

    private void handleVisualizationDataY(VisualizationData data)
    {
        yData = data;
        dataUpdated = true;
        visualizeDataY(data);
    }

    private void handleVisualizationDataZ(VisualizationData data)
    {
        zData = data;
        dataUpdated = true;
        visualizeDataZ(data);
    }

    protected virtual void visualizeAmbisonicData(
        VisualizationData wData,
        VisualizationData xData,
        VisualizationData yData,
        VisualizationData zData)
    { }

    protected virtual void visualizeDataW(VisualizationData data) { }
    protected virtual void visualizeDataX(VisualizationData data) { }
    protected virtual void visualizeDataY(VisualizationData data) { }
    protected virtual void visualizeDataZ(VisualizationData data) { }
}
