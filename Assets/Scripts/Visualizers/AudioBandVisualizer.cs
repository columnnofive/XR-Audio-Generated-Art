using UnityEngine;

public abstract class AudioBandVisualizer: AudioVisualizer
{
    [SerializeField]
    public int band = 0;

    private void OnValidate()
    {
        //Constrain band to valid value
        int bandCount = AudioVisualizationController.getBandCount(VisualizationController.analysisMode);
        if (band < 0)
            band = 0;
        else if (band > bandCount - 1)
            band = bandCount - 1;
    }
}
