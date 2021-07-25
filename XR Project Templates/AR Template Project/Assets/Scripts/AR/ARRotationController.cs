using SonAR.Location;
using UnityEngine;

public class ARRotationController : MonoBehaviour
{
    [SerializeField]
    private Transform user;

    [SerializeField]
    private RotationController gpsRotController;

    [SerializeField]
    private ARVisualizationModeController arVizModeCtrllr;

    private void Start()
    {
        arVizModeCtrllr.OnModeChange += handleModeChange;
    }

    private void handleModeChange(ARVisualizationModeController.ARVisualizationMode newMode)
    {
        if (newMode == ARVisualizationModeController.ARVisualizationMode.Camera)
        {
            gpsRotController.enabled = false;
            user.localRotation = Quaternion.identity;
        }
        else //ARVisualizationMode.Map
        {
            //Rotate rotation controller to match user's rotation in camera mode
            gpsRotController.transform.rotation = user.rotation;
            gpsRotController.enabled = true;
        }
    }
}
