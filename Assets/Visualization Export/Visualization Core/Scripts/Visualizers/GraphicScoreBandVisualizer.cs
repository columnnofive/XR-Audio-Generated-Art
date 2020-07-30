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

    #endregion Editor Accessible Setters
}
