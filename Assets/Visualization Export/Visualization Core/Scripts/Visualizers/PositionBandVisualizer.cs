using UnityEngine;

public class PositionBandVisualizer : AudioBandVisualizer
{
    [SerializeField]
    private float speedFactor = 1f;

    [Header("Direction")]

    [SerializeField]
    private float minDirectionChangeAngle = -90f;

    [SerializeField]
    private float maxDirectionChangeAngle = 90f;

    [SerializeField, Range(0, 1)]
    private float directionChangeThreshold = 0.5f;

    [Header("Movement Inner Boundaries")]

    [SerializeField]
    private float innerMovementRadius = 15f;

    [Header("Movement Outer Boundaries")]

    [SerializeField]
    private float minOuterMovementRadius = 35f;

    [SerializeField]
    private float maxOuterMovementRadius = 65f;

    private float movementRadius;
    private Vector3 movementDirection = Vector3.forward;

    protected override void visualizeData(VisualizationData data)
    {
        scaleMovementRadius(data.amplitude);
        controlDirection(data.amplitude);
        move(data.audioBands[band]);
    }

    private void scaleMovementRadius(float amplitude)
    {
        float radiusInterpolation = (maxOuterMovementRadius - minOuterMovementRadius) * amplitude;
        movementRadius = minOuterMovementRadius + radiusInterpolation;
    }

    private void controlDirection(float amplitude)
    {
        if (amplitude > directionChangeThreshold)
        {
            float angle = Random.Range(minDirectionChangeAngle, maxDirectionChangeAngle);            
            movementDirection = Quaternion.AngleAxis(angle, getRandomAxis()) * movementDirection;
        }
    }

    private void move(float bandAmplitude)
    {
        if (enforceOuterBoundary()) //Within outer boundary
            enforceInnerBoundary(); //Check inner boundary

        Vector3 translation = bandAmplitude * speedFactor * movementDirection;
        transform.Translate(translation, Space.Self);        
    }

    private bool enforceOuterBoundary()
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
            movementDirection = Quaternion.AngleAxis(180f, getRandomAxis()) * movementDirection;

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
            movementDirection = Quaternion.AngleAxis(180f, getRandomAxis()) * movementDirection;

            //Inside inner boundary
            return false;
        }

        //Outside inner boundary
        return true;
    }
}
