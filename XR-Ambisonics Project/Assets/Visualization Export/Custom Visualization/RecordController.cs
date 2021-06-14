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
        Time,
        LastClip,
        ClipIndex
    }

    public RecordAt recordAt;
    //[ConditionaField]

    private void Awake()
    {
        recordScript = GetComponent<Record>();
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
        
        foreach (AnimationState state in anim)
        {
            //do nothing for now
        }

        Debug.Log(anim.GetClipCount());
        //AddClip();
    }

    private void LoadAnimations()
    {
        int i = 2;
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

    private void AddClip()
    {
        string clipName = (anim.GetClipCount() + 1).ToString();
        AnimationClip animationClip = new AnimationClip();
        animationClip.name = clipName;
        animationClip.legacy = true;
        //add clip in assets
        AssetDatabase.CreateAsset(animationClip, directoryPath + "/" + animationClip.name + ".anim");
        //add clip in recorder
        recordScript.clip = animationClip;
        //add clip in Animation component
        anim.AddClip(animationClip, clipName);
        RecordClip();
    }

    private void RecordClip()
    {
        recordScript.enabled = true;
        StartCoroutine(turnOff());
    }

    IEnumerator turnOff()
    {
        yield return new WaitForSeconds(5);
        recordScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
