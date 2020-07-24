using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementConstraintMode
{
    SpecificAngle,
    RandomAngleInRange
}

[System.Serializable]
public class PositionVisualizer
{
    public float speedFactor = 0.1f;

    [Header("Direction")]

    public float minDirectionChangeAngle = -90f;

    public float maxDirectionChangeAngle = 90f;

    [Range(0, 1)]
    public float directionChangeThreshold = 0.5f;

    [Tooltip("Determines how movement is constrained to the movement radius.")]
    public MovementConstraintMode movementConstraintMode = MovementConstraintMode.SpecificAngle;

    [Tooltip("Angle used for the Specific Angle movement constraint mode.")]
    public float movementConstraintAngle = 180f;

    [Header("Movement Inner Boundary")]

    public float innerMovementRadius = 0.03f;

    [Header("Movement Outer Boundaries")]

    public float minOuterMovementRadius = 0.05f;

    public float maxOuterMovementRadius = 0.2f;

    private float movementRadius;
    private Vector3 movementDirection = Vector3.forward;

    public void scaleMovementRadius(float amplitude)
    {
        float radiusInterpolation = (maxOuterMovementRadius - minOuterMovementRadius) * amplitude;
        movementRadius = minOuterMovementRadius + radiusInterpolation;
    }

    public void controlDirection(float amplitude)
    {
        if (amplitude > directionChangeThreshold)
        {
            float angle = Random.Range(minDirectionChangeAngle, maxDirectionChangeAngle);
            movementDirection = Quaternion.AngleAxis(angle, AudioVisualizerBase.getRandomAxis()) * movementDirection;
        }
    }

    public void move(Transform transform, float amplitude)
    {
        if (enforceOuterBoundary(transform)) //Within outer boundary
            enforceInnerBoundary(transform); //Check inner boundary

        Vector3 translation = amplitude * speedFactor * movementDirection;
        transform.Translate(translation, Space.Self);
    }

    private bool enforceOuterBoundary(Transform transform)
    {
        Vector3 toOrigin = Vector3.zero - transform.localPosition;
        float distanceFromOrigin = toOrigin.magnitude;

        //Outside outer boundary
        if (distanceFromOrigin > movementRadius)
        {
            Vector3 normalizedToOrigin = toOrigin.normalized;
            float distanceToMove = distanceFromOrigin - movementRadius;
            Vector3 boundaryTranslation = distanceToMove * normalizedToOrigin;
            transform.localPosition = transform.localPosition + boundaryTranslation;

            //Switch directions
            float directionChange = getMovementConstraintAngle();
            movementDirection = Quaternion.AngleAxis(directionChange, AudioVisualizerBase.getRandomAxis()) * movementDirection;

            //Outside outer boundary
            return false;
        }

        //Within outer boundary
        return true;
    }

    private bool enforceInnerBoundary(Transform transform)
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
            float directionChange = getMovementConstraintAngle();
            movementDirection = Quaternion.AngleAxis(directionChange, AudioVisualizerBase.getRandomAxis()) * movementDirection;

            //Inside inner boundary
            return false;
        }

        //Outside inner boundary
        return true;
    }

    private float getMovementConstraintAngle()
    {
        if (movementConstraintMode == MovementConstraintMode.SpecificAngle)
            return movementConstraintAngle;
        else //RandomAngleInRange
            return Random.Range(minDirectionChangeAngle, maxDirectionChangeAngle);
    }
}
