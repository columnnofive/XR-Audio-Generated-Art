using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SonAR.Location
{
    public static class LocationInterpreter
    {
        /// <summary>
        /// Gets the Unity world space position of the location relative to the configured Origin location.
        /// </summary>
        /// <param name="location">Location to convert to cartesian coordinates.</param>
        /// <returns>Vector3 representation of location relative to origin.</returns>
        public static Vector3 geoToWorldPosition(GeoLocationData location, AbstractMap mapboxMap)
        {
            Vector2d latlon = new Vector2d(location.latitude, location.longitude);
            return mapboxMap.GeoToWorldPosition(latlon, false);
        }

        /// <summary>
        /// Gets latitude and longitude of a position in Unity world space.
        /// </summary>
        /// <param name="position">Position to conver to latitude and longitude.</param>
        /// <returns>GeoLocationData representation of position.</returns>
        public static GeoLocationData worldToGeoLocation(Vector3 position, AbstractMap mapboxMap)
        {
            Vector2d location = mapboxMap.WorldToGeoPosition(position);
            return new GeoLocationData(location.x, location.y);
        }

        /// <summary>
        /// Determines if the two location data structures refer to the same location.
        /// </summary>
        /// <param name="geoLocationData"></param>
        /// <param name="mapboxLocation"></param>
        /// <returns></returns>
        public static bool locationsEqual(GeoLocationData geoLocationData, Vector2d mapboxLocation)
        {
            bool latitudeEqual = geoLocationData.latitude == mapboxLocation.x;
            bool longitudeEqual = geoLocationData.longitude == mapboxLocation.y;
            return latitudeEqual && longitudeEqual;
        }
    }

    /// <summary>
    /// Serializable and editable data structure representing a geographical location.
    /// </summary>
    [System.Serializable]
    public struct GeoLocationData
    {
        public double latitude;
        public double longitude;

        public GeoLocationData(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public GeoLocationData(LocationInfo location)
        {
            latitude = location.latitude;
            longitude = location.longitude;
        }
    }
}