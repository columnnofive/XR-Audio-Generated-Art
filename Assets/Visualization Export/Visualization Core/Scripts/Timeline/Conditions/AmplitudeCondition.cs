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
    private float requiredRepetitions;
    
    [SerializeField, ReadOnlyField]
    private float repetitions = 0f;

    private bool initialized = false;
    private bool conditionMet = false;

    public override bool isMet()
    {
        if (!initialized)
        {
            StartCoroutine(checkConditions());
        }

        return conditionMet;
    }

    private IEnumerator checkConditions()
    {
        while (!conditionMet)
        {
            float amplitude = amplitudeMode == AmplitudeMode.BandAverage ? 
                audioData.amplitude : audioData.audioBands[band];

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
