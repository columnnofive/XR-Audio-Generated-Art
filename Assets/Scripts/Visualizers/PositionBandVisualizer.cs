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

    [Header("Movement Contraints")]

    [SerializeField]
    private float minMovementRadius = 35f;

    [SerializeField]
    private float maxMovementRadius = 65f;

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
        float radiusInterpolation = (maxMovementRadius - minMovementRadius) * amplitude;
        movementRadius = minMovementRadius + radiusInterpolation;
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
        Vector3 toOrigin = Vector3.zero - transform.localPosition;
        float distanceFromOrigin = toOrigin.magnitude;

        if (distanceFromOrigin > movementRadius)
        {
            Vector3 normalizedToOrigin = toOrigin.normalized;
            float distanceToMove = distanceFromOrigin - movementRadius;
            Vector3 boundaryTranslation = distanceToMove * normalizedToOrigin;
            transform.localPosition = transform.localPosition + boundaryTranslation;

            //Switch directions
            movementDirection = Quaternion.AngleAxis(180f, getRandomAxis()) * movementDirection;
        }

        Vector3 translation = bandAmplitude * speedFactor * movementDirection;
        transform.Translate(translation, Space.Self);        
    }
}
