using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonAR.Location;

namespace SonAR.SonicMapping
{
    [System.Serializable]
    public class GeoAudio
    {
        [SerializeField, HideInInspector]
        public int id = 0;

        [SerializeField]
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        [SerializeField, HideInInspector]
        internal GeoLocationData location;
        public GeoLocationData Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        public enum GeoAudioType
        {
            Static,
            Dynamic
        }

        [SerializeField, Tooltip("Static: Does not automatically configure audio source and does not control scale." +
            " | Dynamic: Configures an audio source on the visualizer and scales it based on audio source range.")]
        private GeoAudioType type;
        public GeoAudioType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GeoAudio))
                return false;

            GeoAudio audio = (GeoAudio)obj;
            return id == audio.id;
        }
    }
}