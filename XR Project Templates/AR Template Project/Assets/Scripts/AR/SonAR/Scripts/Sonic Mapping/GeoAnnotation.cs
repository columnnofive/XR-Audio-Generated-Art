using SonAR.Location;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonAR.SonicMapping
{
    [System.Serializable]
    public class GeoAnnotation
    {
        [SerializeField, HideInInspector]
        public int id = 0;

        [SerializeField]
        private string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
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

        public override bool Equals(object obj)
        {
            if (!(obj is GeoAnnotation))
                return false;

            GeoAnnotation annotation = (GeoAnnotation)obj;
            return id == annotation.id;
        }
    }
}