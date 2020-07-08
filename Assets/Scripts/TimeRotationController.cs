using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRotationController : AudioVisualizerBase
{
    [SerializeField]
    private float speed = 1f;

    private void Update()
    {
        transform.localRotation *= Quaternion.AngleAxis(Time.deltaTime * speed, getRandomAxis());
    }
}
