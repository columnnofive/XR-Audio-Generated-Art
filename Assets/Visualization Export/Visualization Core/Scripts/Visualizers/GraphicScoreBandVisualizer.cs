using UnityEngine;

public class GraphicScoreBandVisualizer : AudioBandVisualizer
{
    [SerializeField]
    private GraphicScoreAudioVisualizer visualizer;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!visualizer.rend || visualizer.rend.transform != transform)
        {
            visualizer.rend = GetComponent<Renderer>();
        }

        if (visualizer.rend && visualizer.rend.sharedMaterial)
        {
            if (visualizer.rend.sharedMaterial != visualizer.material) //Material changed
            {
                visualizer.material = visualizer.rend.sharedMaterial;
                visualizer.displacementTexField.shader = visualizer.material.shader;
                visualizer.displacementAmountField.shader = visualizer.material.shader;
            }
        }
        else
        {
            visualizer.material = null;
            visualizer.displacementTexField.shader = null;
            visualizer.displacementAmountField.shader = null;
        }

        //Constrain to positive value
        if (visualizer.displacementStreamSize < 0)
            visualizer.displacementStreamSize = 0;
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
