using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class LineBandVisualizer : AudioBandVisualizer
{
    [SerializeField]
    private LineVisualizer lineVisualizer;

    private void OnValidate()
    {
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
        if (lineVisualizer.controlWidth)
            lineVisualizer.setWidth(data.audioBands[band], visualizerScaleFactor);

        if (lineVisualizer.controlColor)
            lineVisualizer.setColor(data.audioBands[band]);
    }
}
