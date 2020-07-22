using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizerBase : MonoBehaviour
{
    /// <summary>
    /// Factor representing the transform's scale in world space.
    /// </summary>
    protected float visualizerScaleFactor
    {
        get
        {
            return (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3;
        }
    }

    private static Vector3[] axes = new Vector3[]
    {
        Vector3.right,
        Vector3.up,
        Vector3.forward
    };

    public static Vector3 getRandomAxis()
    {
        int axisIndex = Random.Range(0, 2);
        return axes[axisIndex];
    }
}
