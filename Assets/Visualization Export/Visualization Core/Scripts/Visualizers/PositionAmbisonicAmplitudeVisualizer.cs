using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionAmbisonicAmplitudeVisualizer : AmbisonicAudioVisualizer
{
    public float speedFactor = 1f;

    [Range(0, 1)]
    public float directionChangeThresholdX = 0.5f;

    [Range(0, 1)]
    public float directionChangeThresholdY = 0.5f;

    [Range(0, 1)]
    public float directionChangeThresholdZ = 0.5f;

    [Header("Movement Inner Boundaries")]
    
    public float innerMovementRadius = 15f;

    [Header("Movement Outer Boundaries")]
    
    public float minOuterMovementRadius = 35f;
    
    public float maxOuterMovementRadius = 65f;

    private float movementRadius;
    
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
        float radiusInterpolation = (maxOuterMovementRadius - minOuterMovementRadius) * amplitude;
        movementRadius = minOuterMovementRadius + radiusInterpolation;
    }

    private void move()
    {
        if (enforceOuterBoundary()) //Within outer boundary
            enforceInnerBoundary(); //Check inner boundary

        Vector3 translation = directionModifier * movementDirection * speedFactor;
        transform.Translate(translation, Space.Self);
    }

    private bool enforceOuterBoundary()
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

            //Outside outer boundary
            return false;
        }

        //Within outer boundary
        return true;
    }

    private bool enforceInnerBoundary()
    {
        Vector3 toOrigin = Vector3.zero - transform.localPosition;
        float distanceFromOrigin = toOrigin.magnitude;

        //Inside inner boundary
        if (distanceFromOrigin < innerMovementRadius)
        {
            Vector3 normalizedToOrigin = toOrigin.normalized;
            float distanceToMove = innerMovementRadius - distanceFromOrigin;
            Vector3 boundaryTranslation = distanceToMove * -normalizedToOrigin; //Move away from the origin
            transform.localPosition = transform.localPosition + boundaryTranslation;

            //Switch directions
            directionModifier = Quaternion.AngleAxis(180f, getRandomAxis());

            //Inside inner boundary
            return false;
        }

        //Outside inner boundary
        return true;
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
