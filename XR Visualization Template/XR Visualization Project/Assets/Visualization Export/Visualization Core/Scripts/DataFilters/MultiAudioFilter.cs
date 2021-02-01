using UnityEngine;

public abstract class MultiAudioFilter : DataFilter
{
    [SerializeField]
    protected AudioVisualizationController filterDataSource;

    protected VisualizationData filterData { get; private set; }

    protected bool filterDataSet
    {
        get
        {
            return filterData.audioBands != null;
        }
    }

    protected virtual void OnEnable()
    {
        filterDataSource.setVisualizationListener(setFilterData);
        filterDataSource.enable();
    }

    protected virtual void OnDisable()
    {
        filterDataSource.disable();
    }

    private void setFilterData(VisualizationData data)
    {
        filterData = data;
    }

    #region Editor Accessible Setters

    public void setAudioBandDataType(AudioBandDataType dataType)
    {
        filterDataSource.audioBandDataType = dataType;
    }

    public void setAmplitudeDataType(AmplitudeDataType dataType)
    {
        filterDataSource.amplitudeDataType = dataType;
    }

    #endregion Editor Accessible Setters
}
