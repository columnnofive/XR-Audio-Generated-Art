using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class LineInstance : MonoBehaviour
{
    Animation anim;
    bool animationStarted = false;

    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void PlayAnimation(AnimationClip ac, SpectrumAnalyzer sa, int mode)
    {
        anim.clip = ac;
        
        if(mode == 0) //Amplitude Visualizer
        {
            transform.GetComponent<LineAmplitudeVisualizer>().VisualizationController.analyzer = sa;
            transform.GetComponent<LineAmplitudeVisualizer>().enabled = true;
        }    
        else if(mode == 1) //Band Visualizer
        {
            transform.GetComponent<LineBandVisualizer>().VisualizationController.analyzer = sa;
            transform.GetComponent<LineBandVisualizer>().enabled = true;
        }

        anim.AddClip(ac, ac.name);
        anim.Play();
        
        
        animationStarted = true;
    }

    private void Update()
    {
        if(animationStarted && !anim.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }

}
