using SonAR.Location;
using SonAR.SonicMapping;
using UnityEngine;

public class ARPositionController : MonoBehaviour
{
    [SerializeField]
    private SonicMapController sonicMapCtrllr;

    [SerializeField]
    private Transform user;

    [SerializeField]
    private PositionController gpsPosController;

    [SerializeField]
    private ARVisualizationModeController arVizModeCtrllr;

    [SerializeField, Tooltip("Distance from an audio visualizer in meters at which movement will be switched to AR tracking only.")]
    private float fineMovementThreshold = 4f;

    private float nearestVisualizer = float.PositiveInfinity;

    private void Start()
    {
        arVizModeCtrllr.OnModeChange += handleModeChange;
    }

    private void handleModeChange(ARVisualizationModeController.ARVisualizationMode newMode)
    {
        if (newMode == ARVisualizationModeController.ARVisualizationMode.Camera)
        {
            //Reset local position from any prior AR movement
            user.localPosition = Vector3.zero;
        }
    }

    //Find nearest audio visualizer
    private void refreshDistance()
    {
        float nearest = float.PositiveInfinity;

        foreach (Transform visualizer in sonicMapCtrllr.GeoAudioObjs)
        {
            float distance = Vector3.Distance(user.position, visualizer.position);
            if (distance < nearest)
                nearest = distance;
        }

        nearestVisualizer = nearest;
    }

    private void controlPos()
    {
        if (arVizModeCtrllr.mode == ARVisualizationModeController.ARVisualizationMode.Camera)
        {
            refreshDistance();

            //Within fine movement threshold
            if (nearestVisualizer <= fineMovementThreshold)
            {
                gpsPosController.enabled = false; //Stop gps movement, too imprecise
            }
            else
            {
                gpsPosController.enabled = true;
            }
        }
        else //ARVisualizationMode.Map | Ensure gps movement is enabled
        {
            gpsPosController.enabled = true;
        }
    }
    
    private void Update()
    {
        controlPos();
    }
}
