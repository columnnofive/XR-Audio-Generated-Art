#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TimeLineBase : SaveLoad
{
    [HideInInspector]
    public List<Clip> timeLine = new List<Clip>();

    [ReadOnlyField]
    [SerializeField]
    private List<float> animationsStartTimes = new List<float>();

    protected override void Awake()
    {
        base.Awake();
        LoadTimeLine();
    }

    //Public functions

    public void LoadTimeLine()
    {
        timeLine = LoadList();
        animationsStartTimes = LoadTimesList();
    }

    public AnimationClip Insert(float time)
    {
        //add to assets
        string uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(getAnimationsPath() + "/anim.anim");
        AnimationClip anim = new AnimationClip();
        anim.legacy = true;
        AssetDatabase.CreateAsset(anim, uniqueFileName);
        //add to timeline
        string actualName = uniqueFileName.Substring(getAnimationsPath().Length + 1, uniqueFileName.Length - getAnimationsPath().Length - 6);
        timeLine.Add(new Clip(time, actualName));
        timeLine.Sort(SortByTime);
        SaveList(timeLine);
        return anim;
    }

    public AnimationClip Add()
    {
        //add to assets
        string uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(getAnimationsPath() + "/anim.anim");
        AnimationClip anim = new AnimationClip();
        anim.legacy = true;
        AssetDatabase.CreateAsset(anim, uniqueFileName);
        //add to timeline
        string actualName = uniqueFileName.Substring(getAnimationsPath().Length + 1, uniqueFileName.Length - getAnimationsPath().Length - 6);
        float time = GetTotalTime();
        timeLine.Add(new Clip(time, actualName));
        SaveList(timeLine);
        return anim;
    }

    public void Copy(AnimationClip animToCopy, float time)
    {
        //add to assets
        string uniqueFileName = AssetDatabase.GenerateUniqueAssetPath(getAnimationsPath() + "/anim.anim");
        AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(animToCopy), uniqueFileName);
        //add to timeline
        string actualName = uniqueFileName.Substring(getAnimationsPath().Length + 1, uniqueFileName.Length - getAnimationsPath().Length - 6);
        timeLine.Add(new Clip(time, actualName));
        timeLine.Sort(SortByTime);
        SaveAndLoad();
    }

    public void Modify(int index, float newTime)
    {
        timeLine[index] = new Clip(newTime, timeLine[index].name);
        timeLine.Sort(SortByTime);
        SaveAndLoad();
    }    

    //return false if action wasn't complete succesfully
    public bool Offset(int startIndex, int endIndex, float offset)
    {
        try
        {
            for(int i = startIndex; i <= endIndex; i++)
            {
                timeLine[i] = new Clip((timeLine[i].time + offset), timeLine[i].name);
            }
            SaveAndLoad();
            return true;
        }catch(System.Exception e)
        {
            return false;
        }
    }
    
    public void RemoveAt(int index)
    {
        AssetDatabase.DeleteAsset(getClipPath(timeLine[index].name));
        timeLine.RemoveAt(index);
        SaveAndLoad();
    }

    public void RemoveAll()
    {
        ClearAnimationsDirectory();
        timeLine.Clear();
        SaveAndLoad();
    }

    

    public float GetTotalTime()
    {
        float totalLenght = 0;
        Debug.Log(timeLine.Count);
        if (timeLine.Count > 0)
        {
            totalLenght = GetAnimationClipLenght(timeLine[timeLine.Count-1].name) + timeLine[timeLine.Count-1].time;
        }
        return totalLenght;
    }

    public int GetIndexAtTime(float time)
    {
        int index = 0;
        while (timeLine[index].time != time)
            index++;
        return index;
    }

    public string GetNameAtIndex(int index)
    {
        try
        {
            return timeLine[index].name;
        }
        catch (System.Exception e)
        {
            return "";
        }
    }

    public AnimationClip GetAnimationClip(string name)
    {
        return (AnimationClip)AssetDatabase.LoadAssetAtPath(getClipPath(name), (typeof(Object)));
    }

    public float GetAnimationClipLenght(string name)
    {
        AnimationClip ac = GetAnimationClip(name);
        return ac.length;
    }

    public void RemoveScaleProperty(string name)
    {
        AnimationClip anim = GetAnimationClip(name);
        anim.SetCurve("", typeof(Transform), "m_LocalScale", null);
    }

    //Private Functions
    private int SortByTime(Clip x, Clip y)
    {
        return x.time.CompareTo(y.time);
    }

    private void SaveAndLoad()
    {
        SaveList(timeLine);
        timeLine = LoadList();
        animationsStartTimes = LoadTimesList();
        AssetDatabase.Refresh();
    }
}
#endif