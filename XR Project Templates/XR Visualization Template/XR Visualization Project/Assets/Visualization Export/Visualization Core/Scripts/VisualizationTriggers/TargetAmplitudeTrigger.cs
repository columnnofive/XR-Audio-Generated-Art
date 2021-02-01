using UnityEngine;

[System.Serializable]
public class TargetAmplitudeTrigger : VisualizationTrigger
{
    [Range(0, 1), Tooltip("Amplitude that must be reached for the trigger.")]
    public float targetAmplitude = 0;

    [Tooltip("Number of times the target amplitude must be reached for the trigger to be active.")]
    public int requiredRepetitions = 0;

    [SerializeField, ReadOnlyField]
    private int repetitions = 0;

    public TargetAmplitudeTrigger() { }

    public TargetAmplitudeTrigger(float targetAmplitude, int requiredRepetitions)
    {
        this.targetAmplitude = targetAmplitude;
        this.requiredRepetitions = requiredRepetitions;
    }

    public override bool checkTrigger(float amplitude)
    {
        if (amplitude < targetAmplitude)
            return false;

        //targetAmplitude reached
        repetitions++;

        //Triggered
        if (repetitions >= requiredRepetitions)
        {
            repetitions = 0;
            return true;
        }
        else //Not triggered
            return false;
    }
}
