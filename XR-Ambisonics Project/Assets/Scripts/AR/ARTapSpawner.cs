using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class ARTapSpawner : MonoBehaviour
{
    [SerializeField]
    private ARSessionOrigin arOrigin;

    [SerializeField]
    private ARRaycastManager raycastManager;

    [SerializeField]
    private GameObject placementIndicator;

    [SerializeField]
    private GameObject spawnObjPrefab;

    [SerializeField]
    private ARObjParent arObjParentPrefab;

    [SerializeField, Tooltip("Max number of objects that can be spawned.")]
    private int spawnLimit = 1;

    private int spawned = 0; //Number of objects spawned

    private readonly Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0f);
    
    private Pose spawnPose;
    private List<ARRaycastHit> hits;

    private bool spawnPoseValid = false;
    
    private void Start()
    {
        hits = new List<ARRaycastHit>();
    }
    
    private void Update()
    {
        if (spawned < spawnLimit)
        {
            updateSpawnPose();
            updatePlacementIndicator();
            checkForSpawn();
        }
        else
        {
            if (placementIndicator.activeSelf)
            {
                placementIndicator.SetActive(false);
                Debug.Log("Spawn limit reached");
            }
        }
    }

    private void updateSpawnPose()
    {
        Vector3 screenCenter = arOrigin.camera.ViewportToScreenPoint(viewportCenter);

        hits.Clear();
        if (raycastManager.Raycast(screenCenter, hits))
        {
            if (hits.Count > 0) //Raycast hit a trackable
            {
                spawnPose = hits[0].pose;

                Vector3 camForward = arOrigin.camera.transform.forward;
                Vector3 camBearing = new Vector3(camForward.x, 0, camForward.z).normalized;
                spawnPose.rotation = Quaternion.LookRotation(camBearing);

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
        placementIndicator.SetActive(spawnPoseValid);

        if (spawnPoseValid)
        {
            placementIndicator.transform.position = spawnPose.position;
            placementIndicator.transform.rotation = spawnPose.rotation;
        }
    }

    private void checkForSpawn()
    {
        //Spawn pose valid and screen touched
        if (spawnPoseValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            spawn();
    }

    private void spawn()
    {
        ARObjParent arObjParent = Instantiate(arObjParentPrefab);
        arObjParent.transform.position = spawnPose.position;
        arObjParent.transform.rotation = spawnPose.rotation;

        GameObject spawnedObj = Instantiate(spawnObjPrefab, arObjParent.objManipulator.transform);       

        spawned++;
    }
}
