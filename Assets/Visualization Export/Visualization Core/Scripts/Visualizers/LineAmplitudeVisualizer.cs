using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class LineAmplitudeVisualizer : AudioVisualizer
{
    [SerializeField]
    private LineVisualizer lineVisualizer;

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!lineVisualizer.trailRenderer)
            lineVisualizer.trailRenderer = GetComponent<TrailRenderer>();

        if (lineVisualizer.trailRenderer)
        {
            if (lineVisualizer.trailRenderer.sharedMaterial)
            {
                lineVisualizer.shaderColorField.shader = lineVisualizer.trailRenderer.sharedMaterial.shader;
            }
            else
            {
                lineVisualizer.shaderColorField.shader = null;
            }
        }
    }

    private void Start()
    {
        lineVisualizer.trailRenderer = GetComponent<TrailRenderer>();
    }

    protected override void visualizeData(VisualizationData data)
    {
        lineVisualizer.visualize(data.amplitude, visualizerScaleFactor);
    }

    #region Editor Accessible Setters

    public void setControlWidth(bool value)
    {
        lineVisualizer.controlWidth = value;
    }

    public void setMinWidth(float value)
    {
        lineVisualizer.minWidth = value;
    }

    public void setMaxWidth(float value)
    {
        lineVisualizer.maxWidth = value;
    }

    public void setControlColor(bool value)
    {
        lineVisualizer.controlColor = value;
    }

    public void setColorChangeSpeed(float value)
    {
        lineVisualizer.colorChangeSpeed = value;
    }

    public void setHueMin(float value)
    {
        lineVisualizer.hueMin = Mathf.Clamp(value, 0f, 1f);
    }

    public void setHueMax(float value)
    {
        lineVisualizer.hueMax = Mathf.Clamp(value, 0f, 1f);
    }

    public void setEmissionIntensity(float value)
    {
        lineVisualizer.emissionIntensity = value;
    }

    #endregion Editor Accessible Setters
}
