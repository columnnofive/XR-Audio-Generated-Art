using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAmplitudeVisualizer : AudioVisualizer
{
    public float minScale = 0f;    
    public float maxScale = 1f;

    public VisualizationTriggerAuthoring scaleChangeTrigger =
        new VisualizationTriggerAuthoring(new ContinuousTrigger());

    protected override void visualizeData(VisualizationData data)
    {
        setScale(data.amplitude);
    }

    private void setScale(float amplitude)
    {
        if (scaleChangeTrigger.trigger.checkTrigger(amplitude))
        {
            float scaleInterpolation = (maxScale - minScale) * amplitude;
            float scale = minScale + scaleInterpolation;
            transform.localScale = scale * Vector3.one;
        }
    }
}
