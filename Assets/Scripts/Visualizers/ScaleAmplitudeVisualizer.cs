using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAmplitudeVisualizer : AudioVisualizer
{
    [SerializeField]
    private float minScale = 0f;

    [SerializeField]
    private float maxScale = 1f;

    private float scale = 0f;

    protected override void visualizeData(VisualizationData data)
    {
        setScale(data.amplitude);
    }

    private void setScale(float amplitude)
    {
        float scaleInterpolation = (maxScale - minScale) * amplitude;
        scale = minScale + scaleInterpolation;
        transform.localScale = scale * Vector3.one;
    }
}
