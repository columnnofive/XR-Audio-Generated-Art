using UnityEngine;

public abstract class DataFilter : MonoBehaviour
{
    public abstract VisualizationData filter(VisualizationData dataToFilter);
}
