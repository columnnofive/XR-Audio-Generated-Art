using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Animate : TimeLineBase
{
    public Object objectInstance;
    public AudioSource audioSource;
    public SpectrumAnalyzer spectrumAnalyzer;

    private int mode = 0; // 0 = amplitude visualizer, 1 = band visualizer
    private int animationIndex = 0;

    [MyBox.ButtonMethod]
    private void PrepareForBuild()
    {
        base.Awake();
        gameObject.AddComponent<AnimateBuild>();
        AnimateBuild ab = GetComponent<AnimateBuild>();
        ab.objectInstance = objectInstance;
        ab.audioSource = audioSource;
        ab.spectrumAnalyzer = spectrumAnalyzer;
        ab.timeLineBuild = LoadTimesList();
        List<AnimationClip> animationClips = new List<AnimationClip>();
        foreach (Clip c in timeLine)
        {
            Debug.Log(c.name);
            animationClips.Add(GetAnimationClip(c.name));
        }
        ab.animationClipsBuild = animationClips;
    }

    protected override void Awake()
    {
        base.Awake();

        if (gameObject.GetComponent<LineAmplitudeVisualizer>() != null)
        {
            gameObject.GetComponent<LineAmplitudeVisualizer>().VisualizationController.analyzer = spectrumAnalyzer;
            if (gameObject.GetComponent<LineAmplitudeVisualizer>().isActiveAndEnabled)
                mode = 0;
        }

        if (gameObject.GetComponent<LineBandVisualizer>() != null)
        {
            gameObject.GetComponent<LineBandVisualizer>().VisualizationController.analyzer = spectrumAnalyzer;
            if (gameObject.GetComponent<LineBandVisualizer>().isActiveAndEnabled)
                mode = 1;
        }

    }

    private void Start()
    {
        if (timeLine.Count == 0 && Application.isPlaying)
        {
            Debug.Log("You are trying to animate a line that has no values. Maybe you meant to record?");
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }

    private void Update()
    {
        if (audioSource.time > timeLine[animationIndex].time)
        {
            Transform temporary = (Instantiate(objectInstance, Vector3.zero, Quaternion.identity, this.transform) as GameObject).transform; //instantiate Line and save Transform so we can access LineInstance
            temporary.name = animationIndex.ToString();
            LineInstance LineInstance = temporary.GetComponent<LineInstance>(); //access LineInstance
            LineInstance.PlayAnimation(GetAnimationClip(timeLine[animationIndex].name), spectrumAnalyzer, mode);      //play animation from LineInstance
            //Debug.Log("Playing animation index: " + (animationIndex));
            //Debug.Log(audioSource.time.ToString());
            animationIndex++;
            if ((animationIndex) >= timeLine.Count)
            {
                this.enabled = false;
            }
        }
    }

    public void TimeChangedManually()
    {
        int newIndex = 0;
        if (audioSource.time > timeLine[timeLine.Count - 1].time)   //disable if position is after the last animation start time
        {
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
            while (audioSource.time > timeLine[newIndex].time)      //find animation that should be plyed at the current time
            {
                newIndex++;
            }
            animationIndex = newIndex;
        }

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);                   //Destroy all children. Makes it easier to debug
        }
    }

}
