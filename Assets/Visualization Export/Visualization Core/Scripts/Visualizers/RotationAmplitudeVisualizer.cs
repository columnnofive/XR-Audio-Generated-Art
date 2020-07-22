using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAmplitudeVisualizer : AudioVisualizer
{
    [SerializeField, Range(0, 1)]
    private float rotationThreshold = 0.5f;

    [SerializeField]
    private float minRotationChange = -45f;

    [SerializeField]
    private float maxRotationChange = 45f;

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
}
