using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAmplitudeVisualizer : AudioVisualizer
{
    public VisualizationTriggerAuthoring rotationChangeTrigger =
        new VisualizationTriggerAuthoring(new TargetAmplitudeTrigger(0.5f, 1));
    
    public float minRotationChange = -45f;    
    public float maxRotationChange = 45f;

    protected override void visualizeData(VisualizationData data)
    {
        rotate(data.amplitude);
    }

    private void rotate(float amplitude)
    {
        if (rotationChangeTrigger.trigger.checkTrigger(amplitude))
        {
            float angle = Random.Range(minRotationChange, maxRotationChange);
            transform.rotation *= Quaternion.AngleAxis(angle, getRandomAxis());
        }
    }
}
