using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAmbisonicAmplitudeVisualizer : AmbisonicAudioVisualizer
{
    [SerializeField]
    private float speedFactor = 1f;

    [SerializeField, Range(0, 1)]
    private float directionChangeThresholdX = 0.5f;

    [SerializeField, Range(0, 1)]
    private float directionChangeThresholdY = 0.5f;

    [SerializeField, Range(0, 1)]
    private float directionChangeThresholdZ = 0.5f;

    [Header("Movement Contraints")]

    [SerializeField]
    private float minMovementRadius = 35f;

    [SerializeField]
    private float maxMovementRadius = 65f;

    private float movementRadius;

    [SerializeField]
    private Vector3 movementDirection = Vector3.forward;
    private Quaternion directionModifier = Quaternion.identity;

    protected override void visualizeAmbisonicData(
        VisualizationData wData,
        VisualizationData xData,
        VisualizationData yData,
        VisualizationData zData)
    {
        scaleMovementRadius(wData.amplitude);
        move();
    }

    private void scaleMovementRadius(float amplitude)
    {
        float radiusInterpolation = (maxMovementRadius - minMovementRadius) * amplitude;
        movementRadius = minMovementRadius + radiusInterpolation;
    }

    private void move()
    {
        Vector3 toOrigin = Vector3.zero - transform.localPosition;
        float distanceFromOrigin = toOrigin.magnitude;

        if (distanceFromOrigin > movementRadius)
        {
            Vector3 normalizedToOrigin = toOrigin.normalized;
            float distanceToMove = distanceFromOrigin - movementRadius;
            Vector3 boundaryTranslation = distanceToMove * normalizedToOrigin;
            transform.localPosition = transform.localPosition + boundaryTranslation;

            //Switch directions
            directionModifier = Quaternion.AngleAxis(180f, getRandomAxis());
        }

        Vector3 translation = directionModifier * movementDirection * speedFactor;
        transform.Translate(translation, Space.Self);
    }

    protected override void visualizeDataX(VisualizationData data)
    {
        //Negative movement
        if (data.amplitude < directionChangeThresholdX)
            movementDirection.x = -data.amplitude;
        else //Positive movement
            movementDirection.x = data.amplitude;
    }

    protected override void visualizeDataY(VisualizationData data)
    {
        //Negative movement
        if (data.amplitude < directionChangeThresholdY)
            movementDirection.y = -data.amplitude;
        else //Positive movement
            movementDirection.y = data.amplitude;
    }

    protected override void visualizeDataZ(VisualizationData data)
    {
        //Negative movement
        if (data.amplitude < directionChangeThresholdZ)
            movementDirection.z = -data.amplitude;
        else //Positive movement
            movementDirection.z = data.amplitude;
    }
}
