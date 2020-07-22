using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBandVisualizerSpawner : MonoBehaviour
{
    [SerializeField]
    private SpectrumAnalyzer analyzer;

    [SerializeField]
    private AnalysisMode analysisMode;

    [SerializeField]
    private Transform visualizerParent;

    [SerializeField]
    private GameObject visualizerPrefab;

    [SerializeField]
    private List<DataFilter> dataFilters = new List<DataFilter>();
    
    private void Start()
    {
        spawnVisualizers();
    }

    private void spawnVisualizers()
    {
        int bands = AudioVisualizationController.getBandCount(analysisMode);

        for (int i = 0; i < bands; i++)
        {
            GameObject visualizer = Instantiate(visualizerPrefab, visualizerParent);
            AudioBandVisualizer[] bandVisualizers = visualizer.GetComponents<AudioBandVisualizer>();

            foreach (AudioBandVisualizer bandVisualizer in bandVisualizers)
            {
                bandVisualizer.band = i;
                bandVisualizer.VisualizationController.analyzer = analyzer;
                bandVisualizer.VisualizationController.analysisMode = analysisMode;
                bandVisualizer.VisualizationController.dataFilters = dataFilters;
                bandVisualizer.VisualizationController.enable();
            }
        }
    }
}
