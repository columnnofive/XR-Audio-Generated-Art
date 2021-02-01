using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimelineEvent : MonoBehaviour
{
    private TimelineCondition[] conditions;
    
    public UnityEvent OnEventFired;

    public bool fired
    {
        get
        {
            return eventState == EventState.Fired;
        }
    }

    private enum EventState
    {
        Inactive, Active, Fired
    }

    private EventState eventState = EventState.Inactive;

    public void activate()
    {
        conditions = GetComponentsInChildren<TimelineCondition>();
        eventState = EventState.Active;
        StartCoroutine(runEvent());
    }

    private IEnumerator runEvent()
    {
        while (eventState == EventState.Active)
        {
            if (conditionsMet())
            {
                eventState = EventState.Fired;
                OnEventFired.Invoke();
                gameObject.SetActive(false);
            }

            yield return null;
        }
    }

    public bool conditionsMet()
    {
        foreach (TimelineCondition condition in conditions)
        {
            if (!condition.isMet())
                return false;
        }
        return true;
    }
}
