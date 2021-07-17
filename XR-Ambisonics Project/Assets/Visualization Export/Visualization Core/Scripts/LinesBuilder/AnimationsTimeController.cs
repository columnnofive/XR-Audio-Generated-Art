#if UNITY_EDITOR
using UnityEngine;

public class AnimationsTimeController : MonoBehaviour
{
    public AudioSource audioSource;

    [Range(0, 1), SerializeField]
    private float clipTimeProportion = 0f;

    private float previousClipTimeProportion = 0f;

    [ReadOnlyField, SerializeField]
    private float currentClipTime = 0f;

    [SerializeField]
    private GameObject animatedObjectsParent;

    private void updateClipTime()
    {
        if (audioSource.clip)
        {
            if (clipTimeProportion != previousClipTimeProportion) //Clip time was set manually
            {
                audioSource.time = (audioSource.clip.length - 1) * clipTimeProportion;
                if (animatedObjectsParent != null)
                    UpdateAnimations();
            }

            else //set clipTime to follow audioSource.time
                clipTimeProportion = audioSource.time / (audioSource.clip.length - 1);

            currentClipTime = audioSource.time; //Debug purposes only

            previousClipTimeProportion = clipTimeProportion;
        }
    }

    private void Update()
    {
        updateClipTime();
    }

    private void UpdateAnimations()
    {
        foreach (Animate animatedObjectScript in animatedObjectsParent.GetComponentsInChildren<Animate>())
        {
            animatedObjectScript.TimeChangedManually();
        }
    }
}
#endif