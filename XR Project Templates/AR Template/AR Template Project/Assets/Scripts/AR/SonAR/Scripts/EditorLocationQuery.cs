using Mapbox.Unity.Map;
using SonAR.Location;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLocationQuery : MonoBehaviour
{
    public AbstractMap mapboxMap;
    public GeoLocationData origin;
    public GeoLocationData location;
    public Vector3 position;

    public enum Mode
    {
        GeoToWorld,
        WorldToGeo
    }

    public Mode mode;

    private void OnValidate()
    {
        mapboxMap.SetCenterLatitudeLongitude(new Mapbox.Utils.Vector2d(origin.latitude, origin.longitude));

        if (mode == Mode.GeoToWorld)
        {
            position = LocationInterpreter.geoToWorldPosition(location, mapboxMap);
        }
        else if (mode == Mode.WorldToGeo)
        {
            location = LocationInterpreter.worldToGeoLocation(position, mapboxMap);
        }
    }
}
