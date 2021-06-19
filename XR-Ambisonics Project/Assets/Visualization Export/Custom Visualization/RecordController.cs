using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[RequireComponent(typeof(Record))]
[RequireComponent(typeof(AnimateLine))]
public class RecordController : SaveLoadList
{
    private Record recordScript;
    public AudioSource audioSource;
    string directoryPath;

    public enum RecordAt
    {
        LastClip,
        ClipIndex,
        Time
    }

    [MyBox.Separator("Recording Options")]
    public RecordAt recordAt = RecordAt.LastClip;
    
    [MyBox.ConditionalField(nameof(recordAt), false, RecordAt.ClipIndex)] public int clipIndex = 0; //if RecordAt.ClipIndex

    [MyBox.ConditionalField(nameof(recordAt), false, RecordAt.Time)] public float clipTime = 0;     //if RecordAt.Time

    private List<float> timeLine;



    private void Awake()
    {
        if(GetComponent<AnimateLine>().isActiveAndEnabled) //prevents from overriding data
        {
            this.enabled = false;
            Debug.Log("RecordController has been disabled because AnimateLine is enabled");
        }
        recordScript = GetComponent<Record>();

        recordScript.enabled = false;
        
        directoryPath = "Assets/Visualization Export/Custom Visualization/" + this.name;

        timeLine = LoadList(this.name);
        
        if (!Directory.Exists(directoryPath)) //Create Directory if it doesn't exist
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    void Start()
    {
        StartRecording();
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
        timeLine.Add(GetAnimationsLenght());                        //add animiation starting time
        EnableRecording(animationClip);                             //give clip to record on
    }

    private void RecordAtClipIndex()
    {
        AnimationClip animationClip = AddClipAsset(clipIndex);      //add clip to assets and return it
        audioSource.time = timeLine[clipIndex];
        EnableRecording(animationClip);                             //give clip to record on
    }

    private void RecordAtTime()
    {
        AnimationClip animationClip = AddClipAtTime();              //add clip to assets and return it + shift animation names and timeLine
        audioSource.time = clipTime;
        EnableRecording(animationClip);                             //give clip to record on
    }

    private float GetAnimationsLenght()
    {
        float animationsLenght = 0;
        if (timeLine.Count > 0)
        {
            AnimationClip lastAnimation = (AnimationClip)AssetDatabase.LoadAssetAtPath(GetClipPath(timeLine.Count - 1), (typeof(Object)));
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
            string path = GetClipPath(i - 2);
            string newName = (i-1).ToString() + ".anim";
            AssetDatabase.RenameAsset(path, newName); //shift anims right by one
        }
        
    }

    private string GetClipPath(int index)
    {
        return directoryPath + "/" + index.ToString() + ".anim";
    }

    private AnimationClip AddClipAsset(int clipIndex)
    {
        string clipName = clipIndex.ToString();
        AnimationClip animationClip = new AnimationClip();
        animationClip.name = clipName;
        animationClip.legacy = true;
        //add clip in assets
        AssetDatabase.CreateAsset(animationClip, directoryPath + "/" + animationClip.name + ".anim");
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
        SaveList(timeLine, this.name);
    }

}
