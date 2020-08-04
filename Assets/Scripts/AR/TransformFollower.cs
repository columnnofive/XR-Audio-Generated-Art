using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    
    private void Update()
    {
        transform.SetPositionAndRotation(target.position, target.rotation);
    }
}
