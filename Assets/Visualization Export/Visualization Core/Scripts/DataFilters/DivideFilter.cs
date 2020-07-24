using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideFilter : MultiAudioFilter
{
    public enum DivisionMode
    {
        DataToFilterByFilterData,
        FilterDataByDataToFilter
    }

    [Tooltip("How the division will be carried out. Data to filter / filter data or filter data / data to filter.")]
    public DivisionMode divisionMode;

    public override VisualizationData filter(VisualizationData dataToFilter)
    {
        if (!filterDataSet) //Filter data not set, don't filter
            return dataToFilter;

        VisualizationData filteredData = dataToFilter;

        if (divisionMode == DivisionMode.DataToFilterByFilterData)
        {
            filteredData.audioBands = divide(dataToFilter.audioBands, filterData.audioBands);
            filteredData.amplitude = dataToFilter.amplitude / filterData.amplitude;
        }
        else // DivisionMode.FilterDataByDataToFilter
        {
            filteredData.audioBands = divide(filterData.audioBands, dataToFilter.audioBands);
            filteredData.amplitude = filterData.amplitude / dataToFilter.amplitude;
        }        

        return filteredData;
    }

    /// <summary>
    /// Divides each element of a by the corresponding b element.
    /// </summary>
    private float[] divide(float[] a, float[] b)
    {
        int length = a.Length < b.Length ? a.Length : b.Length;
        float[] quotient = new float[length];

        for (int i = 0; i < length; i++)
            quotient[i] = a[i] / b[i];

        return quotient;
    }
}
