using UnityEngine;
using UnityEditor.Animations;

public class ClipRecorder : TimeLineBase
{
    public enum RecordAt
    {
        LastClip,
        ClipIndex,
        Time
    }

    public AudioSource audioSource;
    private GameObjectRecorder recorder;
    private AnimationClip clip;
    private bool isPlayingAudio;

    [MyBox.Separator("Recording Options")]
    public RecordAt recordAt = RecordAt.LastClip;
    [MyBox.ConditionalField(nameof(recordAt), false, RecordAt.ClipIndex)] public int clipIndex = 0; //RecordAt.ClipIndex
    [MyBox.ConditionalField(nameof(recordAt), false, RecordAt.Time)] public float clipTime = 0;     //RecordAt.Time

    private void Start()
    {
        if (GetComponent<Animate>().isActiveAndEnabled) //prevents from overriding data
        {
            this.enabled = false;
            Debug.Log("ClipRecorder has been disabled because Animate is enabled");
        }
        else
        {
            audioSource.Pause();
            base.Awake();
            
            gameObject.AddComponent<FollowMouse>();     //Change this script to record with a different device

            // Create recorder and record the script GameObject.
            recorder = new GameObjectRecorder(gameObject);

            // Bind all the Transforms on the GameObject and all its children.
            recorder.BindComponentsOfType<Transform>(gameObject, true);
        }
    }

    private void Update()
    {
        if (!isRecording() && !isPlayingAudio)
            audioSource.Pause();
        if ((Input.GetMouseButtonDown(0) && !isPlayingAudio) || (Input.GetMouseButtonDown(0) && isRecording()))
        {
            if (!isRecording())
                StartRecording();
            else 
                StopRecording(); 
        }
        if (Input.GetMouseButtonDown(1) && !isRecording())
        {
            if (!isPlayingAudio)
                PlayAudio();
            else
                PauseAudio();
        }
    }

    void LateUpdate()
    {
        if (clip == null)
            return;

        // Take a snapshot and record all the bindings values for this frame.
        recorder.TakeSnapshot(Time.deltaTime);
    }

    public void StartRecordingAtLastClip()
    {
        PlayAudio();
        AnimationClip animationClip = Add();            //add clip to assets and get it
        clip = animationClip;                           //Start Recording (watch LateUpdate for more info)
    }

    public void StartRecordingAtTime(float time)
    {
        PlayAudio();
        AnimationClip animationClip = Insert(time);     //add clip to assets and get it
        clip = animationClip;                           //Start Recording (watch LateUpdate for more info)
    }

    public void OverrideClipAtIndex(int index)
    {
        float clipStartTime = timeLine[index].time;     
        RemoveAt(index);                                //delete clip
        StartRecordingAtTime(clipStartTime);            //record new clip
    }

    public void StartRecording()
    {
        switch (recordAt)
        {
            case RecordAt.LastClip: //add
                StartRecordingAtLastClip();
                break;

            case RecordAt.ClipIndex://modify
                OverrideClipAtIndex(clipIndex);
                break;

            case RecordAt.Time:     //insert
                StartRecordingAtTime(clipTime);
                break;
        }
    }

    public bool isRecording()
    {
        return recorder.isRecording;
    }

    private void PlayAudio()
    {
        float time = 0;
        switch (recordAt)
        {
            case RecordAt.LastClip:
                time = GetTotalTime();
                break;

            case RecordAt.ClipIndex://modify
                time = timeLine[clipIndex].time;
                break;

            case RecordAt.Time:     //insert
                time = clipTime;
                break;
        }
        audioSource.time = time;
        audioSource.Play();
        isPlayingAudio = true;
    }

    private void PauseAudio()
    {
        audioSource.Pause();
        isPlayingAudio = false;
    }

    private void StopRecording()
    {
        if (isRecording())
        {
            PauseAudio();
            // Save the recorded session to the clip.
            string name = clip.name;
            recorder.SaveToClip(clip);
            clip = null;
            RemoveScaleProperty(name);
            UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play"); //Stop game execution for now...
        }
    }
}
