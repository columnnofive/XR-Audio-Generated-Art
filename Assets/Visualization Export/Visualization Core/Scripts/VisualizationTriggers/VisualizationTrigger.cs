using UnityEngine;

[System.Serializable]
public class VisualizationTrigger
{
    public virtual bool checkTrigger(float amplitude)
    {
        return true;
    }
}