using UnityEngine;

[System.Serializable]
public abstract class TimelineCondition : MonoBehaviour
{
    public abstract bool isMet();
}
