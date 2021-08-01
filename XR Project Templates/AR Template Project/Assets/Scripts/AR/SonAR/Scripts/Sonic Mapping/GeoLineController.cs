using Mapbox.Unity.Map;
using SonAR.Location;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonAR.SonicMapping
{
    [RequireComponent(typeof(LineRenderer))]
    public class GeoLineController : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public GeoLine geoLine;

        [SerializeField, HideInInspector]
        public LineRenderer lineRenderer;

        [SerializeField, HideInInspector]
        public AbstractMap mapboxMap;

        internal void initialize(AbstractMap mapboxMap)
        {
            this.mapboxMap = mapboxMap;
        }

        /// <summary>
        /// Position line renderer vertices in world space based on geo location.
        /// </summary>
        private void configureLineRenderer()
        {
            Vector3[] positions = new Vector3[geoLine.Positions.Count];

            for (int i = 0; i < geoLine.Positions.Count; i++)
            {
                positions[i] = LocationInterpreter.geoToWorldPosition(geoLine.Positions[i], mapboxMap);
            }

            lineRenderer.SetPositions(positions);
        }
    }
}