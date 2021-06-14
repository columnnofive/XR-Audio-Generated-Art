using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimateLine : MonoBehaviour
{
    private Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play();
        for(int i = 2; i <= anim.GetClipCount(); i++)
        {
            anim.PlayQueued(i.ToString());
        }
        
    }

    
    void Update()
    {
        
    }
}
