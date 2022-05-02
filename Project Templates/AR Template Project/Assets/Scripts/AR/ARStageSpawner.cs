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
    private GameObject AidDirectionInterface;

    [SerializeField]
    private GameObject placeHolder;

    [SerializeField]
    private GameObject spawnObjPrefab;

    [SerializeField]
    private ARObjParent arObjParentPrefab;

    [SerializeField]
    private float distance = 2f;

    [Header("TimeSettings")]
    [SerializeField]
    private float startTime = 16.0f;

    private int sUntilSpawnTime = 0;

    private bool validPosition = false;

    GameObject placeHolderInstance;

    private Pose spawnPose;

    private void Start()
    {
        getSpawnTime();
        StartCoroutine(checkForSpawn());
        StartCoroutine(spawnAtTime());
    }

    private void getSpawnTime()
    {
        //normalize from 24:60 to 24:100
        float decPlaces = startTime % 1;
        startTime = startTime - decPlaces + decPlaces * 100 / 60;

        System.DateTime now = System.DateTime.Now;
        System.DateTime spawnTime = System.DateTime.Today.AddHours(startTime);

        if (now > spawnTime)
        {
            spawnTime = spawnTime.AddDays(1f);
        }

        sUntilSpawnTime = (int)((spawnTime - now).TotalSeconds);
    }

    IEnumerator checkForSpawn()
    {
        while (!validPosition)
        {
            //Spawn pose valid and screen touched
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                updateSpawnPose();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void updateSpawnPose()
    {
        spawnPose.position = arOrigin.camera.transform.forward * distance + arOrigin.camera.transform.position;
        spawnPose.rotation = Quaternion.LookRotation(new Vector3(spawnPose.position.x, 0, spawnPose.position.z).normalized);
        validPosition = true;
        spawn();
    }


    IEnumerator spawnAtTime()
    {
        if (validPosition)
        {
            yield return new WaitForSeconds(sUntilSpawnTime);
            ARObjParent arObjParent = Instantiate(arObjParentPrefab);
            arObjParent.transform.position = spawnPose.position;
            arObjParent.transform.rotation = spawnPose.rotation;
            GameObject spawnedObj = Instantiate(spawnObjPrefab, arObjParent.objManipulator.transform);
            Destroy(placeHolderInstance);
            AidDirectionInterface.SetActive(false);
        }
    }

    private void spawn()
    {
        ARObjParent arObjParent = Instantiate(arObjParentPrefab);
        arObjParent.transform.position = spawnPose.position;
        arObjParent.transform.rotation = spawnPose.rotation;

        GameObject placeHolderInstance = Instantiate(placeHolder, arObjParent.objManipulator.transform);
        placeHolderInstance.SetActive(true);
    }
}
