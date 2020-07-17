using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapSpawner : MonoBehaviour
{
    [SerializeField]
    private ARSessionOrigin arOrigin;

    [SerializeField]
    private ARRaycastManager raycastManager;

    [SerializeField]
    private GameObject placementIndicator;
    
    private Pose spawnPose;
    private List<ARRaycastHit> hitResults;

    private bool spawnPoseValid = false;
    
    private void Start()
    {
        hitResults = new List<ARRaycastHit>();
    }
    
    private void Update()
    {
        Debug.Log("Spawn pose valid: " + spawnPoseValid);

        updateSpawnPose();
        updatePlacementIndicator();
    }

    private void updateSpawnPose()
    {
        Vector3 screenCenter = arOrigin.camera.ViewportToScreenPoint(new Vector3(0.5f, 0, 0.5f));
        //Ray camForward = new Ray(arCam.transform.position, arCam.transform.forward);
        hitResults.Clear();
        if (raycastManager.Raycast(screenCenter, hitResults))
        {
            if (hitResults.Count > 0) //Raycast hit a trackable
            {
                spawnPose = hitResults[0].pose;

                Debug.Log("[XR-Ambisonics] Hit trackable, updating spawn pose");

                spawnPoseValid = true;
            }
            else
                spawnPoseValid = false;
        }
        else
            spawnPoseValid = false;
    }

    private void updatePlacementIndicator()
    {
        //placementIndicator.SetActive(spawnPoseValid);

        if (spawnPoseValid)
        {
            placementIndicator.transform.position = spawnPose.position;
            placementIndicator.transform.rotation = spawnPose.rotation;
        }
    }
}
