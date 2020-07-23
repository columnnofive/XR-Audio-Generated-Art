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
    [SerializeField]
    private float speedFactor = 0.1f;

    [Header("Direction")]

    [SerializeField]
    private float minDirectionChangeAngle = -90f;

    [SerializeField]
    private float maxDirectionChangeAngle = 90f;

    [SerializeField, Range(0, 1)]
    private float directionChangeThreshold = 0.5f;

    [SerializeField, Tooltip("Determines how movement is constrained to the movement radius.")]
    private MovementConstraintMode movementConstraintMode = MovementConstraintMode.SpecificAngle;

    [SerializeField, Tooltip("Angle used for the Specific Angle movement constraint mode.")]
    private float movementConstraintAngle = 180f;

    [Header("Movement Inner Boundary")]

    [SerializeField]
    private float innerMovementRadius = 0.03f;

    [Header("Movement Outer Boundaries")]

    [SerializeField]
    private float minOuterMovementRadius = 0.05f;

    [SerializeField]
    private float maxOuterMovementRadius = 0.2f;

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
