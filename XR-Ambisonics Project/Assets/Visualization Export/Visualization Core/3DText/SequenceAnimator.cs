using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceAnimator : MonoBehaviour
{
    public float WaitBetween = 0.15f;

    public float speed = 1f;

    List<Animator> animators;

    void Start()
    {
        //get animator components from children
        animators = new List<Animator>(GetComponentsInChildren<Animator>());

        //change animation speed
        foreach (var animator in animators)
        {
            animator.speed = speed;
        }

        //start animation loop
        StartCoroutine(DoWaveAnimation());
    }

    IEnumerator DoWaveAnimation()
    {
        while (true)
        {
            foreach(var animator in animators)
            {
                animator.SetTrigger("triggerWave");
                yield return new WaitForSeconds(WaitBetween);
            }
        }
    }
}
