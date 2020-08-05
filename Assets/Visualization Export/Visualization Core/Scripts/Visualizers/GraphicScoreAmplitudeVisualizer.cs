using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicScoreAmplitudeVisualizer : AudioVisualizer
{
    [SerializeField]
    private GraphicScoreAudioVisualizer visualizer;

    protected override void OnValidate()
    {
        base.OnValidate();
        visualizer.OnValidate(gameObject);
    }

    private void Start()
    {
        visualizer.rend = GetComponent<Renderer>();
        visualizer.material = visualizer.rend.material;

        visualizer.initialize();
    }

    protected override void visualizeData(VisualizationData data)
    {
        visualizer.visualize(data.amplitude);
    }

    #region Editor Accessible Setters

    public void setControlDisplacementAmount(bool value)
    {
        visualizer.controlDisplacementAmount = value;
    }

    public void setDisplacementAmountFactor(float value)
    {
        visualizer.displacementAmountFactor = value;
    }

    public void setDisplacementStreamSize(int value)
    {
        visualizer.displacementStreamSize = value;
    }

    #endregion Editor Accessible Setters
}
