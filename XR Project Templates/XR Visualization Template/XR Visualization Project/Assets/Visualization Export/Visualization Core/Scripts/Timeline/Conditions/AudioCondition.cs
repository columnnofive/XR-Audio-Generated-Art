using UnityEngine;

public abstract class AudioCondition : TimelineCondition
{
    [SerializeField]
    protected AudioVisualizationController audioDataController;

    protected VisualizationData audioData { get; private set; }

    protected virtual void OnEnable()
    {
        audioDataController.setVisualizationListener(visualizeData);
        audioDataController.enable();
    }

    protected virtual void OnDisable()
    {
        audioDataController.disable();
    }

    private void visualizeData(VisualizationData data)
    {
        audioData = data;
    }
}
