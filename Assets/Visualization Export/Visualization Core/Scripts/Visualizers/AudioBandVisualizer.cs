using UnityEngine;

public abstract class AudioBandVisualizer: AudioVisualizer
{
    [SerializeField]
    protected int band = 0;

    protected override void OnValidate()
    {
        constrainBandValue(ref band);
    }

    /// <summary>
    /// Constrains the band to a valid value
    /// </summary>
    private void constrainBandValue(ref int band)
    {
        if (VisualizationController != null)
        {
            //Constrain band to valid value
            int bandCount = AudioVisualizationController.getBandCount(VisualizationController.analysisMode);

            band = Mathf.Clamp(band, 0, bandCount - 1);
        }
    }

    public void setBand(int band)
    {
        constrainBandValue(ref band);
        this.band = band;
    }
}
