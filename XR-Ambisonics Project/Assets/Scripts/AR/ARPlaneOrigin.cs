using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneOrigin : MonoBehaviour
{
    [SerializeField]
    private ARRaycastManager raycastManager;

    private Ray downRay = new Ray(Vector3.zero, Vector3.down);
    private Ray upRay = new Ray(Vector3.zero, Vector3.up);
    
    private List<ARRaycastHit> hits;

    //private bool positioned = false;

    private void Start()
    {
        hits = new List<ARRaycastHit>();
    }

    private void Update()
    {
        updatePosition();
    }

    private bool updatePlaneOriginPose(out Pose planeOriginPose)
    {
        downRay.origin = transform.position;
        hits.Clear();

        //Raycast hit a trackable below transform
        if (raycastManager.Raycast(downRay, hits) && hits.Count > 0)
        {
            planeOriginPose = hits[0].pose;
            return true;
        }
        else
        {
            upRay.origin = transform.position;
            hits.Clear();

            //Raycast hit a trackable above transform
            if (raycastManager.Raycast(upRay, hits) && hits.Count > 0)
            {
                planeOriginPose = hits[0].pose;
                return true;
            }
            else
            {
                planeOriginPose = default;
                return false;
            }
        }
    }

    private void updatePosition()
    {
        //Plane origin pose is valid, position transform
        if (updatePlaneOriginPose(out Pose planeOriginPose))
        {
            transform.position = planeOriginPose.position;
        }

        //if (!positioned)
        //{
        //    //Plane origin pose is valid, position transform
        //    if (updatePlaneOriginPose(out Pose planeOriginPose))
        //    {
        //        transform.position = planeOriginPose.position;
        //        positioned = true;
        //    }
        //}
    }
}
