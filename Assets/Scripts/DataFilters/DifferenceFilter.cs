using UnityEngine;

public class DifferenceFilter : MultiAudioFilter
{
    public override VisualizationData filter(VisualizationData dataToFilter)
    {
        if (!filterDataSet) //Filter data not set, don't filter
            return dataToFilter;

        VisualizationData filteredData = dataToFilter;

        filteredData.audioBands = subtract(filterData.audioBands, dataToFilter.audioBands);
        filteredData.amplitude = absDifference(filterData.amplitude, dataToFilter.amplitude);

        return filteredData;
    }

    private float[] subtract(float[] a, float[] b)
    {
        int length = a.Length < b.Length ? a.Length : b.Length;
        float[] difference = new float[length];

        for (int i = 0; i < length; i++)
            difference[i] = absDifference(a[i], b[i]);

        return difference;
    }

    /// <summary>
    /// Gets the absolute value of the difference between a and b.
    /// </summary>
    private float absDifference(float a, float b)
    {
        return Mathf.Abs(a - b);
    }
}
