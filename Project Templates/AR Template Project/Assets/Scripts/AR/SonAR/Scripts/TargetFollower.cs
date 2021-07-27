using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFollower : MonoBehaviour
{
    [Tooltip("Transform to follow.")]
    public Transform target;

    [Header("Translation")]

    public bool matchPosition = true;

    [Tooltip("Speed at which the target will be followed if matchPosition is checked.")]
    [Range(0f, 1f)]
    public float translationSpeed = 1f;

    [Header("Rotation")]

    public bool matchRotation = true;

    [Tooltip("Speed at which the rotation of the target will be matched if matchRotation is checked.")]
    [Range(0f, 1f)]
    public float rotationSpeed = 0.5f;

    [HideInInspector]
    public float rotationOffset = 0f;

    //Follow target position in xz-plane
    private void followPosition()
    {
        if (target && matchPosition)
        {
            Vector3 position = transform.position;
            Vector3 targetPosition = target.position;
            Vector3 lerpPosition = Vector3.Lerp(position, targetPosition, translationSpeed);
            Vector3 newPosition = new Vector3(lerpPosition.x, position.y, lerpPosition.z);
            transform.position = newPosition;
        }
    }

    //Follow target rotation around y-axis
    private void followRotation()
    {
        if (target && matchRotation)
        {
            Vector3 targetRotationEuler = target.eulerAngles;
            Vector3 currentRotationEuler = transform.eulerAngles;
            Quaternion yAxisRotation = Quaternion.Euler(currentRotationEuler.x, targetRotationEuler.y + rotationOffset, currentRotationEuler.z);
            Quaternion lerpRotation = Quaternion.Lerp(transform.rotation, yAxisRotation, rotationSpeed);
            transform.rotation = lerpRotation;
        }
    }

    /// <summary>
    /// Sets the transform position to the target position.
    /// </summary>
    public void snapToTargetPosition()
    {
        //Move to target
        if (target)        
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
    }

    /// <summary>
    /// Sets the transform rotation to the target rotation with rotation offset.
    /// </summary>
    public void snapToTargetRotation()
    {
        //Rotate to target
        if (target)
        {
            Vector3 targetRotationEuler = target.eulerAngles;
            Vector3 currentRotationEuler = transform.eulerAngles;
            Quaternion yAxisRotation = Quaternion.Euler(currentRotationEuler.x, targetRotationEuler.y + rotationOffset, currentRotationEuler.z);
            transform.rotation = yAxisRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        followPosition();
        followRotation();
    }
}