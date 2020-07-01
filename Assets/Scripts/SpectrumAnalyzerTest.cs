using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumAnalyzerTest : MonoBehaviour
{
    public AudioSource audioSource;
    public SpectrumAnalyzer analyzer;
    public GameObject visualizerPrefab;
    public float spacing = 0.1f;
    public float scaleFactor = 1f;

    [Range(0, 1)]
    public float clipStartTimeFraction = 0;

    private Transform[] visualizers;

    private void Start()
    {
        audioSource.time = audioSource.clip.length * clipStartTimeFraction;

        spawnVisualizers();
    }

    private void spawnVisualizers()
    {
        visualizers = new Transform[analyzer.FrequencyBandCount];

        float startX = transform.position.x - (visualizers.Length / 2);
        float posZ = transform.position.z;

        for (int i = 0; i < visualizers.Length; i++)
        {
            Transform visualizer = Instantiate(visualizerPrefab, transform).transform;
            visualizer.position = new Vector3(startX + i + spacing * i, 0, posZ);
            visualizers[i] = visualizer;
        }
    }

    private void setPositionsFromScale()
    {
        for (int i = 0; i < visualizers.Length; i++)
        {
            Transform visualizer = visualizers[i];

            Vector3 pos = visualizer.position;
            visualizer.position = new Vector3(pos.x, visualizer.localScale.y / 2, pos.z);
        }
    }

    private void scaleVisualizers()
    {
        for (int i = 0; i < visualizers.Length; i++)
        {
            Transform visualizer = visualizers[i];

            float scaleY = scaleFactor * analyzer.FrequencyBands[i];
            Vector3 scale = visualizer.localScale;
            visualizer.localScale =  new Vector3(scale.x, scaleY, scale.z);
        }
    }
    
    private void Update()
    {
        scaleVisualizers();
        setPositionsFromScale();
    }
}
