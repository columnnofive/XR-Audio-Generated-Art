using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeRootParent : MonoBehaviour
{
    [SerializeField, Tooltip("Specifies any transforms to ignore when parenting to the runtime parent.")]
    private List<Transform> rootTransformsToIgnore;

    [SerializeField]
    private List<string> tagsToIgnore;

    /// <summary>
    /// Root transform parent of this transform.
    /// </summary>
    private Transform rootParent;

    /// <summary>
    /// All root transforms in hierarchy that are not parents of this.
    /// </summary>
    private Transform[] rootTransforms;

    private void Awake()
    {
        rootParent = transform.getRootParent();
        rootTransformsToIgnore.Add(rootParent); //Make sure rootParent is ignored

        //Get root transforms to make as children
        rootTransforms = getSceneRootTransforms(rootTransformsToIgnore);

        //Make root transforms children of this transform
        rootTransforms.setParents(transform);
    }

    private Transform[] getSceneRootTransforms(List<Transform> ignoredRootTransforms)
    {
        List<GameObject> rootGOs = new List<GameObject>(SceneManager.GetActiveScene().GetRootGameObjects());
        List<Transform> rootTransforms = new List<Transform>();

        //Remove all ignored root transforms from list
        foreach (Transform ignoredRoot in ignoredRootTransforms)
            rootGOs.Remove(ignoredRoot.gameObject);
        
        foreach (GameObject root in rootGOs)
        {
            //Only add to root transforms if tag is not ignored
            if (!tagsToIgnore.Contains(root.tag))
                rootTransforms.Add(root.transform);
        }

        return rootTransforms.ToArray();
    }

    /// <summary>
    /// Maintains world transform values in local space of parent.
    /// </summary>
    //private void setLocalFromWorld(Transform[] transforms)
    //{
    //    foreach (Transform worldTransform in transforms)
    //    {
    //        worldTransform.setLocalFromWorld();
    //    }
    //}
}
