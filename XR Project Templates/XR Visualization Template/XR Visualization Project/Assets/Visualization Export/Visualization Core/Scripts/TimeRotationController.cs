using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRotationController : AudioVisualizerBase
{
    [SerializeField]
    private float speed = 1f;

    public enum RotationAxis
    {
        WorldX, WorldY, WorldZ,
        LocalX, LocalY, LocalZ
    }

    [SerializeField]
    private RotationAxis rotationAxis;

    private void Update()
    {
        transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * speed, getRotationAxis());
    }

    private Vector3 getRotationAxis()
    {
        switch (rotationAxis)
        {
            case RotationAxis.WorldX:
                return transform.InverseTransformDirection(Vector3.right);
            case RotationAxis.WorldY:
                return transform.InverseTransformDirection(Vector3.up);
            case RotationAxis.WorldZ:
                return transform.InverseTransformDirection(Vector3.forward);
            case RotationAxis.LocalX:
                return Vector3.right;
            case RotationAxis.LocalY:
                return Vector3.up;
            case RotationAxis.LocalZ:
                return Vector3.forward;
            default:
                return Vector3.up;
        }
    }
}
