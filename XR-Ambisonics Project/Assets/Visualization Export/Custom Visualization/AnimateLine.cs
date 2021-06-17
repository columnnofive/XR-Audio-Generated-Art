using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(Animation))]
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
    private int animationIndex = 1;

    private void Start()
    {
        timeLine = GetComponent<AnimationsTimeLine>().TimeLine;
        //StartCoroutine(Spawn());
    }

    private void Update()
    {
        if(audioSource.time > timeLine[animationIndex - 1])
        {
            Transform temporary = (Instantiate(LineInstance, Vector3.zero, Quaternion.identity, this.transform) as GameObject).transform; //instantiate Line and save Transform so we can access LineInstance
            temporary.name = animationIndex.ToString();
            LineInstance instance = temporary.GetComponent<LineInstance>(); //access LineInstance
            instance.PlayAnimation(GetAnimation(animationIndex), spectrumAnalyzer, Mode == mode.AmplitudeVisualizer ? 0 : 1);      //play animation from LineInstance
            Debug.Log("Playing animation index: " + (animationIndex-1));
            Debug.Log(audioSource.time.ToString());
            animationIndex++;
            if((animationIndex) >= timeLine.Count)
            {
                this.enabled = false;
            }
        }
    }

    IEnumerator Spawn()
    {
        int lastPlayedAnimation = 1;
        while (lastPlayedAnimation <= timeLine.Count)
        {
            Transform temporary = (Instantiate(LineInstance, Vector3.zero, Quaternion.identity, this.transform) as GameObject).transform; //instantiate Line and save Transform so we can access LineInstance
            temporary.name = lastPlayedAnimation.ToString();
            LineInstance instance = temporary.GetComponent<LineInstance>(); //access LineInstance
            instance.PlayAnimation(GetAnimation(lastPlayedAnimation), spectrumAnalyzer, Mode == mode.AmplitudeVisualizer ? 0 : 1);      //play animation from LineInstance
            lastPlayedAnimation++;
            if(lastPlayedAnimation == 2)
            {
                yield return new WaitForSeconds(timeLine[lastPlayedAnimation - 2]);
            }
            else
            {
                Debug.Log("Starting to wait: " + (timeLine[lastPlayedAnimation - 2] - timeLine[lastPlayedAnimation - 3]) + "s");
                yield return new WaitForSeconds(timeLine[lastPlayedAnimation-2] - timeLine[lastPlayedAnimation - 3]); //-1 because vectors are funny, -1 because we just incremented the variable.
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
