using SonAR.Location;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonAR.SonicMapping
{
    [System.Serializable]
    public class GeoLine
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
        private List<GeoLocationData> positions;
        public List<GeoLocationData> Positions
        {
            get
            {
                return positions;
            }
            set
            {
                positions = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GeoLine))
                return false;

            GeoLine line = (GeoLine)obj;
            return id == line.id;
        }
    }
}