using UnityEngine;

public class BlendFilter : MultiAudioFilter
{
    [Range(0, 1),
     Tooltip("How much weight is given to filter data source. 0 ignores filter data | 1 is full filter data.")]
    public float blendWeight = 0.5f;

    public override VisualizationData filter(VisualizationData dataToFilter)
    {
        if (!filterDataSet) //Filter data not set, don't filter
            return dataToFilter;

        if (blendWeight == 0f) //Don't filter
            return dataToFilter;
        else if (blendWeight == 1f) //Use full filter data
            return filterData;

        VisualizationData filteredData = dataToFilter;

        filteredData.audioBands = blend(dataToFilter.audioBands, filterData.audioBands, blendWeight);
        filteredData.amplitude = blend(dataToFilter.amplitude, filterData.amplitude, blendWeight);

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
            blended[i] = blend(a[i], b[i], blendWeight);

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
