using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VisualizationTriggerAuthoring
{
    public enum TriggerType
    {
        Continuous,
        TargetAmplitude
    }

    public TriggerType triggerType;

    [HideInInspector]
    public VisualizationTrigger trigger;
    
    public ContinuousTrigger continuousTrigger;
    
    public TargetAmplitudeTrigger targetAmplitudeTrigger;

    public VisualizationTriggerAuthoring()
    {
        triggerType = TriggerType.Continuous;
        continuousTrigger = new ContinuousTrigger();
        targetAmplitudeTrigger = new TargetAmplitudeTrigger();
    }

    public VisualizationTriggerAuthoring(ContinuousTrigger continuousTrigger)
    {
        triggerType = TriggerType.Continuous;
        this.continuousTrigger = continuousTrigger;
        targetAmplitudeTrigger = new TargetAmplitudeTrigger();
    }

    public VisualizationTriggerAuthoring(TargetAmplitudeTrigger targetAmplitudeTrigger)
    {
        triggerType = TriggerType.TargetAmplitude;
        continuousTrigger = new ContinuousTrigger();
        this.targetAmplitudeTrigger = targetAmplitudeTrigger;
    }
}
