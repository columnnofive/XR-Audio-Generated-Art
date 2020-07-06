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

    public enum VisualizeMode
    {
        Bands8,
        Bands64
    }
    public VisualizeMode visualizeMode;

    private int bands;

    private void Start()
    {
        audioSource.time = audioSource.clip.length * clipStartTimeFraction;

        if (visualizeMode == VisualizeMode.Bands8)
        {
            bands = 8;
            analyzer.OnAnalyzeBands8.addListener(handleSpectralAnalysis);
        }
        else //Bands64
        {
            bands = 64;
            analyzer.OnAnalyzeBands64.addListener(handleSpectralAnalysis);
        }

        Debug.Log(bands);

        spawnVisualizers();
    }

    private void spawnVisualizers()
    {
        visualizers = new Transform[bands];

        float halfVisualizersCount = visualizers.Length / 2;
        float startX = transform.position.x - (halfVisualizersCount + spacing * halfVisualizersCount);
        float posZ = transform.position.z;

        for (int i = 0; i < visualizers.Length; i++)
        {
            Transform visualizer = Instantiate(visualizerPrefab, transform).transform;
            visualizer.position = new Vector3(startX + i + spacing * i, 0, posZ);
            visualizers[i] = visualizer;
        }
    }

    private void handleSpectralAnalysis(SpectralAnalysisData data)
    {
        scaleVisualizers(data.audioBands);
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

    private void scaleVisualizers(float[] audioBands)
    {
        for (int i = 0; i < visualizers.Length; i++)
        {
            Transform visualizer = visualizers[i];

            float scaleY = scaleFactor * audioBands[i];
            Vector3 scale = visualizer.localScale;
            visualizer.localScale =  new Vector3(scale.x, scaleY, scale.z);
        }

        setPositionsFromScale();
    }
}
