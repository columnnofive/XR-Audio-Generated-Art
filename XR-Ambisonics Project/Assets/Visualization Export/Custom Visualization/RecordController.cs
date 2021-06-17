using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[RequireComponent(typeof(Record))]
[RequireComponent(typeof(Animation))]
public class RecordController : MonoBehaviour
{
    private Animation anim;
    private Record recordScript;
    string directoryPath;

    
    public enum RecordAt
    {
        LastClip,
        ClipIndex
    }

    [MyBox.Separator("Recording Options")]
    public RecordAt recordAt = RecordAt.LastClip;
    
    [MyBox.ConditionalField(nameof(recordAt), false, RecordAt.ClipIndex)] public int ClipIndex = 0;

    [MyBox.Separator("Debugging Only")]
    public float GetClipAtTime = 0;
    [ReadOnlyField]
    public int ClipAtTime = 0;



    private void Awake()
    {
        recordScript = GetComponent<Record>();
        recordScript.enabled = false;
        anim = GetComponent<Animation>();
        directoryPath = "Assets/Visualization Export/Custom Visualization/" + this.name;
        //Create Directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        LoadAnimations();
    }

    void Start()
    {
        StartRecording();
    }

    private void StartRecording()
    {
        switch (recordAt)
        {
            case RecordAt.LastClip:
                RecordAtLastClip();
                    break;
            case RecordAt.ClipIndex:
                RecordAtClipIndex();
                    break;
        }
    }

    private void RecordAtLastClip()
    {
        int clipName = anim.GetClipCount() + 1; //Assign index to last index recorded + 1
        AddClip(clipName);
        EnableRecording();
    }

    private void RecordAtClipIndex()
    {
        AddClip(ClipIndex);
        EnableRecording();
    }

    private void LoadAnimations()
    {
        int i = 1;
        while (File.Exists(GetClipPath(i)))
        {
            Object currentAnimationClip = AssetDatabase.LoadAssetAtPath(GetClipPath(i), (typeof(Object)));
            anim.AddClip((AnimationClip)currentAnimationClip, i.ToString());
            i++;
        }
    }

    private string GetClipPath(int index)
    {
        return directoryPath + "/" + index.ToString() + ".anim";
    }

    private void AddClip(int clipIndex)
    {
        string clipName = clipIndex.ToString();
        AnimationClip animationClip = new AnimationClip();
        animationClip.name = clipName;
        animationClip.legacy = true;
        //add clip in assets
        AssetDatabase.CreateAsset(animationClip, directoryPath + "/" + animationClip.name + ".anim");
        //add clip in recorder
        recordScript.clip = animationClip;
        //add clip in Animation component
        anim.AddClip(animationClip, clipName);
    }

    private void EnableRecording()
    {
        recordScript.enabled = true;
    }
}
