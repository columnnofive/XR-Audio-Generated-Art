using UnityEngine;

public class ARVisualizationModeController : MonoBehaviour
{
    [SerializeField]
    private bool showCameraView = true;

    [SerializeField]
    private GameObject[] cameraViewObjs;

    [SerializeField]
    private GameObject[] mapViewObjs;

    public enum ARVisualizationMode
    {
        Camera,
        Map
    }

    /// <summary>
    /// Current visualization mode.
    /// </summary>
    public ARVisualizationMode mode
    {
        get
        {
            return showCameraView ? ARVisualizationMode.Camera : ARVisualizationMode.Map;
        }
    }

    public delegate void ARVisualizationModeChange(ARVisualizationMode newMode);
    public event ARVisualizationModeChange OnModeChange;

    private void OnValidate()
    {
        handleModeChange();
    }

    private void Start()
    {
        handleModeChange();
    }

    public void setShowCameraView(bool value)
    {
        showCameraView = value;
        handleModeChange();
    }

    private void handleModeChange()
    {
        foreach (GameObject camObj in cameraViewObjs)
            camObj.SetActive(showCameraView);

        foreach (GameObject mapObj in mapViewObjs)
            mapObj.SetActive(!showCameraView);

        OnModeChange?.Invoke(mode);
    }
}
