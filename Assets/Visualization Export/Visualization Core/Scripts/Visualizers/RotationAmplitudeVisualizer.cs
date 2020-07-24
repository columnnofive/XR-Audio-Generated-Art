using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAmplitudeVisualizer : AudioVisualizer
{
    [Range(0, 1)]
    public float rotationThreshold = 0.5f;
    
    public float minRotationChange = -45f;
    
    public float maxRotationChange = 45f;

    protected override void visualizeData(VisualizationData data)
    {
        rotate(data.amplitude);
    }

    private void rotate(float amplitude)
    {
        if (amplitude > rotationThreshold)
        {
            float angle = Random.Range(minRotationChange, maxRotationChange);
            transform.rotation *= Quaternion.AngleAxis(angle, getRandomAxis());
        }
    }

    public void setRotationThreshold(float threshold)
    {
        rotationThreshold = Mathf.Clamp(threshold, 0f, 1f);
    }
}
