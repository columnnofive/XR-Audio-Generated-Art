using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class AnimationsTimeLine : MonoBehaviour
{
    public List<float> TimeLine;
    string directoryPath;

    private void Awake()
    {
        directoryPath = "Assets/Visualization Export/Custom Visualization/" + this.name;
    }

    void Update()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        
        int i = TimeLine.Count;
        float totalLenght = TimeLine[TimeLine.Count-1];


        while (File.Exists(GetClipPath(i)) && TimeLine.Count <= i)
        {
            Debug.Log(i);
            AnimationClip currentAnimationClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(GetClipPath(i), (typeof(Object)));
            totalLenght += currentAnimationClip.length;
            TimeLine.Add(totalLenght);
            i++;
        }
    }

    private string GetClipPath(int index)
    {
        return directoryPath + "/" + index.ToString() + ".anim";
    }

}
#endif
