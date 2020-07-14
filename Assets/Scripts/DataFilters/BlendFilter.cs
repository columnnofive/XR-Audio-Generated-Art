using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendFilter : DataFilter
{
    [SerializeField]
    private AudioVisualizationController blendDataSource;

    [SerializeField,
     Range(0, 1),
     Tooltip("How much weight is given to the blend data source. 0 ignores blend data | 1 is full blend data.")]
    private float blendWeight = 0.5f;

    private VisualizationData blendData;

    private void OnEnable()
    {
        blendDataSource.setVisualizationListener(setBlendData);
        blendDataSource.enable();
    }

    private void OnDisable()
    {
        blendDataSource.disable();
    }

    private void setBlendData(VisualizationData data)
    {
        blendData = data;
    }

    public override VisualizationData filterData(VisualizationData dataToBlend)
    {
        if (blendData.audioBands == null) //Blend data not set
            return dataToBlend;

        if (blendWeight == 0f) //Don't filter
            return dataToBlend;
        else if (blendWeight == 1f) //Use full blend data
            return blendData;

        VisualizationData filteredData = dataToBlend;

        filteredData.audioBands = blend(dataToBlend.audioBands, blendData.audioBands, blendWeight);
        filteredData.amplitude = blend(dataToBlend.amplitude, blendData.amplitude, blendWeight);

        return filteredData;
    }

    /// <summary>
    /// Blends values from a and b using blendWeight;
    /// </summary>
    /// <param name="blendWeight">0 - full a data | 1 - full b data</param>
    private float[] blend(float[] a, float[] b, float blendWeight)
    {
        int length = a.Length < b.Length ? a.Length : b.Length;
        float[] blended = new float[length];

        for (int i = 0; i < length; i++)
        {
            blended[i] = blend(a[i], b[i], blendWeight);
        }

        return blended;
    }

    /// <summary>
    /// Blends a and b values using blendWeight;
    /// </summary>
    /// <param name="blendWeight">0 - full a data | 1 - full b data</param>
    private float blend(float a, float b, float blendWeight)
    {
        return a * (1 - blendWeight) + b * blendWeight;
    }
}
