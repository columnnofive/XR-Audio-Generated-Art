using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class ARStageSpawner : MonoBehaviour
{
    [SerializeField]
    private ARSessionOrigin arOrigin;

    [SerializeField]
    private GameObject spawnObjPrefab;

    [SerializeField]
    private ARObjParent arObjParentPrefab;

    [SerializeField]
    private float distance = 1f;




    private bool spawned = false;

    private readonly Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0f);

    private Pose spawnPose;

    private void Start()
    {
        StartCoroutine(checkForSpawn());
        StartCoroutine(debug());
    }

    IEnumerator debug()
    {
        while (true)
        {
            //nothing
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator checkForSpawn()
    {
        while(true)
        {
            updateSpawnPose();
            Debug.Log(Input.touchCount);
            //Spawn pose valid and screen touched
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                spawn();

            yield return new WaitForEndOfFrame();
        }
    }

    private void updateSpawnPose()
    {
        spawnPose.position = arOrigin.camera.transform.forward * distance + arOrigin.camera.transform.position;
        spawnPose.rotation = Quaternion.LookRotation(new Vector3(spawnPose.position.x, 0, spawnPose.position.z).normalized);
        Debug.Log(spawnPose.position);
        Debug.Log(spawnPose.rotation);
    }


    private void spawn()
    {
        ARObjParent arObjParent = Instantiate(arObjParentPrefab);
        arObjParent.transform.position = spawnPose.position;
        arObjParent.transform.rotation = spawnPose.rotation;
        GameObject spawnedObj = Instantiate(spawnObjPrefab, arObjParent.objManipulator.transform);

        spawned = true;
        distance += 1;
    }
}
