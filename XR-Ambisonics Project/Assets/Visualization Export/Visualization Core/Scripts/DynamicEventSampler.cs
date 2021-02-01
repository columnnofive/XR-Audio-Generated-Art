using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Samples for an event only there are event listeners.
/// </summary>
/// <typeparam name="TEventParam"></typeparam>
public abstract class DynamicEventSampler<TEventParam>
{
    public struct EventSampleResult
    {
        public TEventParam eventParam;
        public bool eventOccurred;
    }

    public class DynamicEvent : UnityEvent<TEventParam> { }

    private UnityEvent<TEventParam> onEventFired;
    private List<UnityAction<TEventParam>> listeners;
    private Func<EventSampleResult> eventSampler;

    private Action startListening;
    private Action stopListening;

    public DynamicEventSampler(Func<EventSampleResult> eventSampler)
    {
        onEventFired = new DynamicEvent();
        listeners = new List<UnityAction<TEventParam>>();
        this.eventSampler = eventSampler;
    }

    public void addListener(UnityAction<TEventParam> listener)
    {
        onEventFired.AddListener(listener);
        listeners.Add(listener);
    }

    public void removeListener(UnityAction<TEventParam> listener)
    {
        onEventFired.RemoveListener(listener);
        listeners.Remove(listener);
    }

    public void update()
    {
        if (listeners.Count > 0)
        {
            EventSampleResult result = eventSampler.Invoke();

            //Event ocurred, notify subscribers
            if (result.eventOccurred)
                onEventFired.Invoke(result.eventParam);
        }
    }
}
