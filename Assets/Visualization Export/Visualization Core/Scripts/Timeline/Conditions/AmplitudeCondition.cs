using System.Collections;
using UnityEngine;

public class AmplitudeCondition : AudioCondition
{
    public enum AmplitudeMode
    {
        Band, BandAverage
    }

    [SerializeField]
    private AmplitudeMode amplitudeMode = AmplitudeMode.BandAverage;

    [SerializeField, Tooltip("Audio band to get amplitude from, NOTE: Only used with band amplitude mode.")]
    private int band = 0;

    [SerializeField, Range(0, 1), Tooltip("Amplitude that must be reached for the trigger.")]
    private float targetAmplitude;

    [SerializeField, Tooltip("Number of times the target amplitude must be reached for the trigger to be active.")]
    private int requiredRepetitions;
    
    [SerializeField, ReadOnlyField]
    private int repetitions = 0;

    private bool initialized = false;
    private bool conditionMet = false;

    private void OnValidate()
    {
        if (audioDataController != null)
        {
            //Constrain band to valid value
            int bandCount = AudioVisualizationController.getBandCount(audioDataController.analysisMode);
            if (band < 0)
                band = 0;
            else if (band > bandCount - 1)
                band = bandCount - 1;
        }
    }

    public override bool isMet()
    {
        if (!initialized)
        {
            StartCoroutine(checkConditions());
            initialized = true;
        }

        return conditionMet;
    }

    private IEnumerator checkConditions()
    {
        while (!conditionMet)
        {
            float amplitude = 0f;
            if (amplitudeMode == AmplitudeMode.BandAverage)
                amplitude = audioData.amplitude;
            else if (audioData.audioBands != null)
                amplitude = audioData.audioBands[band];

            if (amplitude >= targetAmplitude)
            {
                //targetAmplitude reached
                repetitions++;

                //Condition met
                if (repetitions == requiredRepetitions)
                {
                    conditionMet = true;
                }
            }

            yield return null;
        }
    }
}
