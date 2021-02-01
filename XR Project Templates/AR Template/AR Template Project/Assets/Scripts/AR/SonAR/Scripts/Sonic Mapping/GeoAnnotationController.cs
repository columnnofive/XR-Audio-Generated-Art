using Mapbox.Unity.Map;
using SonAR.Location;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SonAR.SonicMapping
{
    public class GeoAnnotationController : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public GeoAnnotation geoAnnotation;

        [SerializeField, HideInInspector]
        public TextMeshPro annotation;

        /// <summary>
        /// Last recorded position in world space, used for detecting a position change.
        /// </summary>
        [SerializeField, HideInInspector]
        public Vector3 worldPos;

        [SerializeField, HideInInspector]
        public AbstractMap mapboxMap;

        internal void initialize(AbstractMap mapboxMap)
        {
            this.mapboxMap = mapboxMap;

            positionAnnotation();
        }

        /// <summary>
        /// Position annotation in world space based on geo location.
        /// </summary>
        private void positionAnnotation()
        {
            annotation.rectTransform.position =
                LocationInterpreter.geoToWorldPosition(geoAnnotation.location, mapboxMap);
        }
    }
}