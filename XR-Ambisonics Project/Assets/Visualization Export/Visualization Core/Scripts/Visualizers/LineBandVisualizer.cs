using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class LineBandVisualizer : AudioBandVisualizer
{
    [SerializeField]
    private LineVisualizer lineVisualizer;

    protected override void OnValidate()
    {
        base.OnValidate();
        lineVisualizer.onValidate(gameObject);
    }

    private void Start()
    {
        lineVisualizer.trailRenderer = GetComponent<TrailRenderer>();
    }

    protected override void visualizeData(VisualizationData data)
    {
        lineVisualizer.visualize(data.audioBands[band], visualizerScaleFactor);
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
        lineVisualizer.colorChangeSpeed =value;
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
