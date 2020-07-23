using UnityEngine;

public class PositionAmplitudeVisualizer : AudioVisualizer
{
    [SerializeField]
    private PositionVisualizer positionVisualizer;

    protected override void visualizeData(VisualizationData data)
    {
        positionVisualizer.scaleMovementRadius(data.amplitude);
        positionVisualizer.controlDirection(data.amplitude);
        positionVisualizer.move(transform, data.amplitude);
    }
}
