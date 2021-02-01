using UnityEngine;

public class GraphicScoreBandVisualizer : AudioBandVisualizer
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
        visualizer.visualize(data.audioBands[band]);
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
