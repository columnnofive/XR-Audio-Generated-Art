using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonAR.Location;

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
            gpsRotController.enabled = true;
        }
    }
}
