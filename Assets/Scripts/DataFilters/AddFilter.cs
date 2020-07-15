public class AddFilter : MultiAudioFilter
{
    public override VisualizationData filter(VisualizationData dataToFilter)
    {
        if (!filterDataSet) //Filter data not set, don't filter
            return dataToFilter;

        VisualizationData filteredData = dataToFilter;

        filteredData.audioBands = add(filterData.audioBands, dataToFilter.audioBands);
        filteredData.amplitude = filterData.amplitude + dataToFilter.amplitude;

        return filteredData;
    }

    private float[] add(float[] a, float[] b)
    {
        int length = a.Length < b.Length ? a.Length : b.Length;
        float[] sum = new float[length];

        for (int i = 0; i < length; i++)
            sum[i] = a[i] + b[i];

        return sum;
    }
}
