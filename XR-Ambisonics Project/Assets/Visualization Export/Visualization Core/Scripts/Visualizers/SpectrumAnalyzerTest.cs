using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectrumAnalyzerTest : AudioVisualizer
{
    public GameObject visualizerPrefab;
    public float spacing = 0.1f;
    public float scaleFactor = 1f;
    public float rotationFactor = 1f;

    [Range(0, 1)]
    public float rotationDirectionCutoff = 0.5f;

    private Transform[] visualizers;

    private void Start()
    {
        spawnVisualizers();
    }

    private void spawnVisualizers()
    {
        visualizers = new Transform[VisualizationController.bands];

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

    protected override void visualizeData(VisualizationData data)
    {
        scaleVisualizers(data.audioBands);
        rotateVisualizers(data.amplitude);
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

    private void rotateVisualizers(float amplitude)
    {
        float rotateY = amplitude * rotationFactor;

        if (amplitude < rotationDirectionCutoff)
            rotateY *= -1;

        for (int i = 0; i < visualizers.Length; i++)
        {
            Transform visualizer = visualizers[i];

            visualizer.rotation *= Quaternion.AngleAxis(rotateY, Vector3.up);
        }
    }
}
