using UnityEngine;

public class TimeCondition : TimelineCondition
{
    [SerializeField, Tooltip("Timer length in seconds.")]
    private float timerLength = 0f;
    
    [SerializeField, ReadOnlyField]
    private float timer = 0f;

    private const float NOT_SET = -1f;
    private float previousTime = NOT_SET;

    public override bool isMet()
    {
        if (previousTime == NOT_SET) //Initialize previous time
            previousTime = Time.time;

        if (timer >= timerLength)
            return true;
        else
        {
            float elapsedTime = Time.time - previousTime;
            timer += elapsedTime;
            previousTime = Time.time;

            return false;
        }
    }
}
