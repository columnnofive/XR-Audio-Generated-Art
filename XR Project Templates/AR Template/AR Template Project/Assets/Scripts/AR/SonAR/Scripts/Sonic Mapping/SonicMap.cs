using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonAR.Location;

namespace SonAR.SonicMapping
{
    [System.Serializable]
    public class SonicMap
    {
        [SerializeField]
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            private set
            {
                name = value;
            }
        }

        [SerializeField, Tooltip("Geographic origin of the map.")]
        private GeoLocationData origin;

        /// <summary>
        /// Geographic origin of the map.
        /// </summary>
        public GeoLocationData Origin
        {
            get
            {
                return origin;
            }
            private set
            {
                origin = value;
            }
        }

        [SerializeField, Tooltip("Geographically placed audio clips in this map.")]
        private List<GeoAudio> audio;

        /// <summary>
        /// Geopgrahically placed audio clips in this map.
        /// </summary>
        public List<GeoAudio> Audio
        {
            get
            {
                return audio;
            }
        }

        [SerializeField, Tooltip("Geographically placed text annotations in this map.")]
        private List<GeoAnnotation> annotations;

        /// <summary>
        /// Geopgrahically placed text annotations in this map.
        /// </summary>
        public List<GeoAnnotation> Annotations
        {
            get
            {
                return annotations;
            }
        }

        [SerializeField, Tooltip("Geographically placed lines in this map.")]
        private List<GeoLine> lines;

        /// <summary>
        /// Geographically placed lines in this map.
        /// </summary>
        public List<GeoLine> Lines
        {
            get
            {
                return lines;
            }
        }

        public SonicMap() { }

        public SonicMap(GeoLocationData origin, string name)
        {
            Origin = origin;
        }
    }
}