using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ShaderFloatAmplitudeVisualizer : AudioVisualizer
{
    private Material material;

    [SerializeField, Tooltip("Name of the shader float being set.")]
    private string floatName = "";

    [SerializeField]
    private float min = 0f;

    [SerializeField]
    private float max = 1f;

    [SerializeField]
    private VisualizationAmplitudeTrigger trigger;

    [SerializeField, Tooltip("How long it will take to interpolate to each interpolation target.")]
    private float interpolationSpeed = 1f;

    [SerializeField, Tooltip("Determines if interpolation targets will be randomly generated every time the trigger is fired.")]
    private bool randomizeInterpolationTargets = false;

    [SerializeField, Tooltip("Sets the number of interpolation targets and sets the target values, if not set randomly, that will be interpolated to once the trigger is fired.")]
    private float[] interpolationTargets = new float[2] { 0, 1 };

    private bool interpolating = false;

    private float floatValue
    {
        get
        {
            return material.GetFloat(floatName);
        }
        set
        {
            material.SetFloat(floatName, value);
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < interpolationTargets.Length; i++)
        {
            float target = interpolationTargets[i];

            //Constrain targets to min and max values.
            if (target < min)
                interpolationTargets[i] = min;
            else if (target > max)
                interpolationTargets[i] = max;
        }
    }

    private void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    protected override void visualizeData(VisualizationData data)
    {
        if (!interpolating && trigger.checkTrigger(data.amplitude))
        {
            if (randomizeInterpolationTargets)
                setRandomTargets();

            StartCoroutine(interpolateFloat());
        }
    }

    private IEnumerator interpolateFloat()
    {
        interpolating = true;
        
        float value = 0f;

        int i = 0;
        while (i < interpolationTargets.Length)
        {
            if (!Mathf.Approximately(interpolationTargets[i], value)) //Has not reached target
            {
                float t = interpolationSpeed * Time.deltaTime;
                value = Mathf.Lerp(value, interpolationTargets[i], t);
                floatValue = value;
            }
            else //Reached target
                i++;

            yield return null; //Wait until next frame
        }

        interpolating = false;
    }

    private void setRandomTargets()
    {
        for (int i = 0; i < interpolationTargets.Length; i++)
            interpolationTargets[i] = Random.Range(min, max);
    }
}
