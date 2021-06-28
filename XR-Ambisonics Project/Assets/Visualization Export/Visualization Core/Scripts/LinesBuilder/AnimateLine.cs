using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(AnimationsTimeLine))]
public class AnimateLine : LineIO
{

    public Object LineInstance;
    public AudioSource audioSource;
    public SpectrumAnalyzer spectrumAnalyzer;

    private int mode = 0; // 0 = amplitude visualizer, 1 = band visualizer
    private List<float> timeLine;
    private int animationIndex = 0;

    protected override void Awake()
    {
        base.Awake();

        if (gameObject.GetComponent<RecordController>() != null)
            gameObject.GetComponent<RecordController>().audioSource = audioSource;

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
        timeLine = SaveLoadList.LoadList(this.name);
#if UNITY_EDITOR
        if(timeLine.Count == 0)
        {
            Debug.Log("READ ME. You are trying to Animate the Line. Maybe you were trying to Record? Use the Animate_Record script to change your choice");
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
#endif
    }

    private void Update()
    {
        if(audioSource.time > timeLine[animationIndex])
        {
            Transform temporary = (Instantiate(LineInstance, Vector3.zero, Quaternion.identity, this.transform) as GameObject).transform; //instantiate Line and save Transform so we can access LineInstance
            temporary.name = animationIndex.ToString();
            LineInstance instance = temporary.GetComponent<LineInstance>(); //access LineInstance
            instance.PlayAnimation(GetAnimation(animationIndex), spectrumAnalyzer, mode);      //play animation from LineInstance
            //Debug.Log("Playing animation index: " + (animationIndex));
            //Debug.Log(audioSource.time.ToString());
            animationIndex++;
            if((animationIndex) >= timeLine.Count)
            {
                this.enabled = false;
            }
        }
    }

    private AnimationClip GetAnimation(int index) 
    {
        return (AnimationClip)AssetDatabase.LoadAssetAtPath(getClipPath(index), (typeof(Object)));
    }

    public void TimeChangedManually()
    {
        int newIndex = 0;
        if(audioSource.time > timeLine[timeLine.Count - 1])
        {
            this.enabled = false;
        }
        else
        {
            this.enabled = true;
            while (audioSource.time > timeLine[newIndex])
            {
                newIndex++;
            }
            animationIndex = newIndex;
        }
        
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

}
