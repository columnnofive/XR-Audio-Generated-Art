using System.Collections.Generic;
using UnityEngine;


public class AnimateBuild : MonoBehaviour
{
    public Object objectInstance;
    public AudioSource audioSource;
    public SpectrumAnalyzer spectrumAnalyzer;

    private int mode = 0; // 0 = amplitude visualizer, 1 = band visualizer
    private int animationIndex = 0;

    public List<float> timeLineBuild;
    public List<AnimationClip> animationClipsBuild;

    private void Awake()
    {
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

    private void Update()
    {
        if (audioSource.time > timeLineBuild[animationIndex])
        {
            Transform temporary = (Instantiate(objectInstance, Vector3.zero, Quaternion.identity, this.transform) as GameObject).transform; //instantiate Line and save Transform so we can access LineInstance
            temporary.name = animationIndex.ToString();
            LineInstance LineInstance = temporary.GetComponent<LineInstance>(); //access LineInstance
            LineInstance.PlayAnimation(animationClipsBuild[animationIndex], spectrumAnalyzer, mode);      //play animation from LineInstance
            animationIndex++;
            if ((animationIndex) >= timeLineBuild.Count)
            {
                this.enabled = false;
            }
        }
    }
}
