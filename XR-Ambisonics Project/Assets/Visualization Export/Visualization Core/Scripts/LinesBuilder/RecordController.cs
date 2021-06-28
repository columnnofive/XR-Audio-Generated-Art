using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Record))]
[RequireComponent(typeof(AnimateLine))]
[RequireComponent(typeof(AnimationsTimeLine))]
[RequireComponent(typeof(FollowMouse))]
public class RecordController : LineIO
{
    private Record recordScript;
    [HideInInspector]
    public AudioSource audioSource;

    public enum RecordAt
    {
        LastClip,
        ClipIndex,
        Time
    }

    [MyBox.Separator("Recording Options")]
    public RecordAt recordAt = RecordAt.LastClip;
    
    [MyBox.ConditionalField(nameof(recordAt), false, RecordAt.ClipIndex)] public int clipIndex = 0; //RecordAt.ClipIndex

    [MyBox.ConditionalField(nameof(recordAt), false, RecordAt.Time)] public float clipTime = 0;     //RecordAt.Time

    private List<float> timeLine;
    private bool isRecording = false;
    private bool isPlayingAudio = false;

    protected override void Awake()
    {
        base.Awake();

        timeLine = SaveLoadList.LoadList(this.name);

        recordScript = GetComponent<Record>();
        recordScript.enabled = false;
        
    }

    void Start()
    {
        audioSource.Pause();
        if (GetComponent<AnimateLine>().isActiveAndEnabled) //prevents from overriding data
        {
            audioSource.Play();
            this.enabled = false;
            Debug.Log("RecordController has been disabled because AnimateLine is enabled");
        }
    }

    private void Update()
    {
        if (!isRecording && !isPlayingAudio)
            audioSource.Pause();
        if (Input.GetMouseButtonDown(0))
        {
            if (!isRecording)
            {
                isRecording = true;
                StartRecording();
            }
            else //Stop game execution, look OnDisable() for more infomations
            {
                EditorApplication.ExecuteMenuItem("Edit/Play");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!isPlayingAudio)
            {
                isPlayingAudio = true;
                StartAudioSample();
            }
            else
            {
                isPlayingAudio = false;
                audioSource.Pause();
            }
        }
    }

    private void StartAudioSample()
    {
        switch (recordAt)
        {
            case RecordAt.LastClip:
                audioSource.time = GetAnimationsLenght();
                audioSource.Play();
                break;

            case RecordAt.ClipIndex:
                audioSource.time = timeLine[clipIndex];
                audioSource.Play();
                break;

            case RecordAt.Time:
                audioSource.time = clipTime;
                audioSource.Play();
                break;
        }
    }

    private void StartRecording()
    {
        switch (recordAt)
        {
            case RecordAt.LastClip: //add
                RecordAtLastClip();
                break;

            case RecordAt.ClipIndex://modify
                RecordAtClipIndex();
                break;

            case RecordAt.Time:     //insert
                RecordAtTime();
                break;
        }
    }

    private void RecordAtLastClip()
    {
        AnimationClip animationClip = AddClipAsset(timeLine.Count); //add clip to assets and return it
        audioSource.time = GetAnimationsLenght();
        audioSource.Play();
        timeLine.Add(GetAnimationsLenght());                        //add animiation starting time
        EnableRecording(animationClip);                             //give clip to record on
    }

    private void RecordAtClipIndex()
    {
        AnimationClip animationClip = AddClipAsset(clipIndex);      //add clip to assets and return it
        audioSource.time = timeLine[clipIndex];
        audioSource.Play();
        EnableRecording(animationClip);                             //give clip to record on
    }

    private void RecordAtTime()
    {
        AnimationClip animationClip = AddClipAtTime();              //add clip to assets and return it + shift animation names and timeLine
        audioSource.time = clipTime;
        audioSource.Play();
        EnableRecording(animationClip);                             //give clip to record on
    }

    private float GetAnimationsLenght()
    {
        float animationsLenght = 0;
        if (timeLine.Count > 0)
        {
            AnimationClip lastAnimation = (AnimationClip)AssetDatabase.LoadAssetAtPath(base.getClipPath(timeLine.Count - 1), (typeof(Object)));
            float lastAnimationLenght = lastAnimation.length;
            animationsLenght = timeLine[timeLine.Count - 1] + lastAnimationLenght;
        }
        return animationsLenght;
    }


    private AnimationClip AddClipAtTime()
    {
        int clipIndex = GetIndexAtTime();
        if(clipIndex == timeLine.Count) //if it is the last index
        {
            timeLine.Add(clipTime); //write clipTime in the right spot
            return AddClipAsset(clipIndex);
        }
        else
        {
            ShiftFromIndex(clipIndex);
            timeLine[clipIndex] = clipTime; //write clipTime in the right spot
            return AddClipAsset(clipIndex);
        }
    }

    private int GetIndexAtTime()
    {
        if (timeLine.Count == 0 || clipTime < timeLine[0]) return 0;

        if (clipTime > timeLine[timeLine.Count-1]) return timeLine.Count;

        int i = 0;
        while (clipTime > timeLine[i])
        {
            i++;
        }
        return i;
    }

    private void ShiftFromIndex(int clipIndex)
    {
        timeLine.Add(0); //add new value at last index
        for(int i = timeLine.Count; i > (clipIndex+1); i--)
        {
            timeLine[i - 1] = timeLine[i - 2]; //shift timeLine right by one
            string path = base.getClipPath(i - 2);
            string newName = (i-1).ToString() + ".anim";
            AssetDatabase.RenameAsset(path, newName); //shift anims right by one
        }
        
    }

    private AnimationClip AddClipAsset(int clipIndex)
    {
        string clipName = clipIndex.ToString();
        AnimationClip animationClip = new AnimationClip();
        animationClip.name = clipName;
        animationClip.legacy = true;
        //add clip in assets
        AssetDatabase.CreateAsset(animationClip, base.getClipPath(clipIndex));
        return animationClip;
    }

    private void EnableRecording(AnimationClip animationClip)
    {
        //add clip in recorder
        recordScript.clip = animationClip;
        //enable record
        recordScript.enabled = true;
    }

    private void OnDisable()
    {
        //Note that if in the future we want to make multiple recordings at once, we should save to the list and reload it after each recorded animation
        SaveLoadList.SaveList(timeLine, this.name); //save all changes applied on the timeLine on Disk
    }

}
