using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Gets the root parent in the hierarchy of a child transform.
    /// </summary>
    public static Transform getRootParent(this Transform child)
    {
        if (child.parent) //Child still has a parent
            return getRootParent(child.parent);
        else //Child is root parent
            return child;
    }

    public static void setParents(this ICollection<Transform> children, Transform parent)
    {
        foreach (Transform child in children)
            child.parent = parent;
    }

    /// <summary>
    /// Sets local position and rotation to world position and rotation.
    /// </summary>
    public static void setLocalFromWorld(this Transform transform)
    {
        transform.localPosition = transform.position;
        transform.localRotation = transform.rotation;
    }
}
