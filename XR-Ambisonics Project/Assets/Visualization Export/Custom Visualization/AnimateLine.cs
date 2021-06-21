using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(AnimationsTimeLine))]
public class AnimateLine : MonoBehaviour
{
    public enum mode
    {
        AmplitudeVisualizer,
        BandVisualizer
    }

    public Object LineInstance;
    public AudioSource audioSource;
    public SpectrumAnalyzer spectrumAnalyzer;
    public mode Mode = mode.AmplitudeVisualizer;

    private List<float> timeLine;
    private int animationIndex = 0;

    private void Start()
    {
        timeLine = SaveLoadList.LoadList(this.name);
    }

    private void Update()
    {
        if(audioSource.time > timeLine[animationIndex])
        {
            Transform temporary = (Instantiate(LineInstance, Vector3.zero, Quaternion.identity, this.transform) as GameObject).transform; //instantiate Line and save Transform so we can access LineInstance
            temporary.name = animationIndex.ToString();
            LineInstance instance = temporary.GetComponent<LineInstance>(); //access LineInstance
            instance.PlayAnimation(GetAnimation(animationIndex), spectrumAnalyzer, Mode == mode.AmplitudeVisualizer ? 0 : 1);      //play animation from LineInstance
            Debug.Log("Playing animation index: " + (animationIndex));
            Debug.Log(audioSource.time.ToString());
            animationIndex++;
            if((animationIndex) >= timeLine.Count)
            {
                this.enabled = false;
            }
        }
    }

    private AnimationClip GetAnimation(int index) {

        string clipName = index.ToString();
        string directoryPath = "Assets/Visualization Export/Custom Visualization/" + this.name;
        string clipPath = directoryPath + "/" + clipName + ".anim";

        return (AnimationClip)AssetDatabase.LoadAssetAtPath(clipPath, (typeof(Object)));
    }

}
