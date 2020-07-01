using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEffectController : MonoBehaviour {

  //  public GameObject scaleObject;
    public Light controlledLight;
    public float scaleCoefficient = 10.0f;
    public AudioSource audioSource;
    int qsamples = 512;
    float previousSum = 0.0f;
    public Color onColor;
    public Color offColor;

    float[] samples;
    float[] freqBands = new float[8];

    bool active = true;

 //   public GameObject[] freqBandObjects;
	// Use this for initialization
	void Start () {
        samples = new float[qsamples];
	}

    public void Toggle()
    {
        if (active)
        {
            audioSource.volume = 0.0f;
            controlledLight.color = offColor;
            active = false;
        }else
        {
            audioSource.volume = 1.0f;
            controlledLight.color = onColor;
            active = true;
        }

    }
	
	// Update is called once per frame
	void Update () {
        float sum = 0.0f;
        if (active)
        {

            audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);
            int count = 0;
            for (int i = 0; i < 8; i++)
            {
                float average = 0;
                int sampleCount = (int)Mathf.Pow(2, i) * 2;
                if (i == 7)
                {
                    sampleCount += 2;
                }
                for (int j = 0; j < sampleCount; j++)
                {
                    average += samples[count] * (count + 1);
                    count++;
                }
                average /= count;
                freqBands[i] = average;
            }

            for (int i = 0; i < 8; i++)
            {
                sum += freqBands[i];
                //  freqBandObjects[i].transform.localScale = new Vector3(1, freqBands[i] * scaleCoefficient*10, 1);
            }
        }// if active
        else
        {
            sum = 0.05f;
        }
        if (sum > previousSum)
        {
            previousSum = sum;
        }
        else
        {
            if (previousSum > 0.02f)
            {
                previousSum -= 0.02f;
            }
        }

    //  gameObject.transform.localScale = Vector3.one * previousSum * scaleCoefficient;
        controlledLight.intensity = previousSum * scaleCoefficient;
    }
}
