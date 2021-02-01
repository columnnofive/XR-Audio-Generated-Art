public class MultiplyFilter : MultiAudioFilter
{
    public override VisualizationData filter(VisualizationData dataToFilter)
    {
        if (!filterDataSet) //Filter data not set, don't filter
            return dataToFilter;

        VisualizationData filteredData = dataToFilter;

        filteredData.audioBands = multiply(filterData.audioBands, dataToFilter.audioBands);
        filteredData.amplitude = filterData.amplitude * dataToFilter.amplitude;

        return filteredData;
    }

    private float[] multiply(float[] a, float[] b)
    {
        int length = a.Length < b.Length ? a.Length : b.Length;
        float[] product = new float[length];

        for (int i = 0; i < length; i++)
            product[i] = a[i] * b[i];

        return product;
    }
}
