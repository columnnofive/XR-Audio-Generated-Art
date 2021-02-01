using UnityEngine;

public class Timeline : MonoBehaviour
{
    private TimelineEvent[] events;
    private int eventIndex = -1;

    private void Start()
    {
        events = GetComponentsInChildren<TimelineEvent>();
        advanceTimeline();
    }

    private void advanceTimeline()
    {
        if (eventIndex >= 0)
        {
            TimelineEvent previousEvent = events[eventIndex];
            previousEvent.OnEventFired.RemoveListener(advanceTimeline);
        }

        //Subscribe to next event
        eventIndex++;        
        if (eventIndex < events.Length)
        {
            TimelineEvent nextEvent = events[eventIndex];
            nextEvent.OnEventFired.AddListener(advanceTimeline);
            nextEvent.activate();
        }
    }
}
