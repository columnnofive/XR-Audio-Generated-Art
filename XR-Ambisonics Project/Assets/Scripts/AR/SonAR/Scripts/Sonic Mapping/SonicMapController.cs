using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonAR.Location;
using TMPro;
using Mapbox.Unity.Map;

namespace SonAR.SonicMapping
{
    public class SonicMapController : MonoBehaviour
    {
        [SerializeField]
        public SonicMap sonicMap;

        [Header("Visualization")]
        
        public const string sonicMapTag = "Sonic Map";

        [SerializeField]
        public GameObject audioVisualizerPrefab;

        [SerializeField]
        public TextMeshPro annotationPrefab;

        [SerializeField]
        public LineRenderer lineRendererPrefab;

        [SerializeField]
        public AbstractMap mapboxMap;

        private Transform[] geoAudioObjs;
        public Transform[] GeoAudioObjs
        {
            get
            {
                if (geoAudioObjs == null)
                {
                    geoAudioObjs = new Transform[geoAudioObjMappings.Count];
                    for (int i = 0; i < geoAudioObjs.Length; i++)
                    {
                        geoAudioObjs[i] = geoAudioObjMappings[i].gameObject.transform;
                    }
                }

                return geoAudioObjs;
            }

        }

        #region Editor Variables

        [SerializeField, HideInInspector]
        public Transform mapParent;

        [SerializeField, HideInInspector]
        public Transform geoAudioParent;

        [SerializeField, HideInInspector]
        public Transform geoAnnotationParent;

        [SerializeField, HideInInspector]
        public Transform geoLineParent;

        [System.Serializable]
        public class GeoAudioObjMapping
        {
            public GameObject gameObject;
            public GeoAudioController controller;
            public AudioSource audioSource;
            public Transform visualizer;
        }

        [SerializeField, HideInInspector]
        public List<GeoAudioObjMapping> geoAudioObjMappings;

        [System.Serializable]
        public class GeoAnnotationObjMapping
        {
            public GameObject gameObject;
            public GeoAnnotationController controller;
            public TextMeshPro textMeshPro;
        }

        [SerializeField, HideInInspector]
        public List<GeoAnnotationObjMapping> geoAnnotationObjMappings;

        [System.Serializable]
        public class GeoLineObjMapping
        {
            public GameObject gameObject;
            public GeoLineController controller;
        }

        [SerializeField, HideInInspector]
        public List<GeoLineObjMapping> geoLineObjMappings;

        #endregion Editor Variables

        private void Start()
        {
            PositionController.Instance.MapboxMap.OnInitialized += initialize;

            PositionController.Instance.initialize(sonicMap.Origin);
        }

        private void initialize()
        {
            foreach(GeoAudioObjMapping geoAudioObjMapping 
                in geoAudioObjMappings)
            {
                geoAudioObjMapping.controller.initialize(
                    PositionController.Instance.MapboxMap
                );
            }

            foreach (GeoAnnotationObjMapping geoAnnotationObjMapping
                in geoAnnotationObjMappings)
            {
                geoAnnotationObjMapping.controller.initialize(
                    PositionController.Instance.MapboxMap
                );
            }

            foreach (GeoLineObjMapping geoLineObjMapping
                in geoLineObjMappings)
            {
                geoLineObjMapping.controller.initialize(
                    PositionController.Instance.MapboxMap
                );
            }
        }
    }
}