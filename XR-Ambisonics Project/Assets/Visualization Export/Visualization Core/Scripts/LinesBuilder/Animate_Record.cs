using UnityEngine;

[RequireComponent(typeof(RecordController))]
[RequireComponent(typeof(AnimateLine))]
[RequireComponent(typeof(FollowMouse))]
public class Animate_Record : MonoBehaviour
{
    public enum Switch
    {
        Animate,
        Record
    }

    public Switch choice = Switch.Record;

    [Range(0, 1), SerializeField]
    private float clipTimeProportion = 0f;

    private float previousClipTimeProportion = 0f;

    [ReadOnlyField, SerializeField]
    private float currentClipTime = 0f;

    private AudioSource audioSource;

    private void Awake()
    {
        //look, I know this doesnt look pretty in the code, but it does in the inspector.
        audioSource = GetComponent<RecordController>().audioSource;

        switch (choice)
        {
            case Switch.Animate:
                GetComponent<AnimateLine>().enabled = true;
                GetComponent<RecordController>().enabled = false;
                GetComponent<FollowMouse>().enabled = false;
                break;
            case Switch.Record:
                GetComponent<RecordController>().enabled = true;
                GetComponent<FollowMouse>().enabled = true;
                GetComponent<AnimateLine>().enabled = false;
                break;
        }
    }

    private void updateClipTime()
    {
        if (audioSource.clip)
        {
            if (clipTimeProportion != previousClipTimeProportion) //Clip time was set manually
            {
                audioSource.time = (audioSource.clip.length - 1) * clipTimeProportion;
                if (gameObject.GetComponent<AnimateLine>() != null)
                    UpdateAnimations();
            }
            else //set clipTime to follow audioSource.time
                clipTimeProportion = audioSource.time / (audioSource.clip.length - 1);

            currentClipTime = audioSource.time; //Debug purposes only

            previousClipTimeProportion = clipTimeProportion;
        }
    }

    private void UpdateAnimations()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("AnimatedLine");
        foreach(GameObject line in lines)
        {
            line.GetComponent<AnimateLine>().TimeChangedManually();
        }
    }

    private void Update()
    {
        updateClipTime();
    }

}
