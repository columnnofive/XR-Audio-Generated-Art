using UnityEngine;

public class EventCondition : TimelineCondition
{
    [SerializeField, Tooltip("This event must fire before the condition is met.")]
    private TimelineEvent eventDependency;

    [SerializeField, ReadOnlyField]
    private bool dependencyEventFired = false;

    public override bool isMet()
    {
        dependencyEventFired = eventDependency.fired;
        return dependencyEventFired;
    }
}
