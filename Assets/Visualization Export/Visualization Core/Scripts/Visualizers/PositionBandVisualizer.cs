using UnityEngine;

public class PositionBandVisualizer : AudioBandVisualizer
{
    [SerializeField]
    private PositionVisualizer positionVisualizer;

    protected override void visualizeData(VisualizationData data)
    {
        positionVisualizer.visualize(data.audioBands[band], transform);
    }

    #region Editor Accessible Setters

    public void setSpeedFactor(float value)
    {
        positionVisualizer.speedFactor = value;
    }

    public void setMinDirectionChangeAngle(float value)
    {
        positionVisualizer.minDirectionChangeAngle = value;
    }

    public void setMaxDirectionChangeAngle(float value)
    {
        positionVisualizer.maxDirectionChangeAngle = value;
    }

    public void setMovementConstraintMode(MovementConstraintMode value)
    {
        positionVisualizer.movementConstraintMode = value;
    }

    public void setMovementConstraintAngle(float value)
    {
        positionVisualizer.movementConstraintAngle = value;
    }

    public void setInnerMovementRadius(float value)
    {
        positionVisualizer.innerMovementRadius = value;
    }

    public void setMinOuterMovementRadius(float value)
    {
        positionVisualizer.minOuterMovementRadius = value;
    }

    public void setMaxOuterMovementRadius(float value)
    {
        positionVisualizer.maxOuterMovementRadius = value;
    }

    #endregion Editor Accessible Setters
}
