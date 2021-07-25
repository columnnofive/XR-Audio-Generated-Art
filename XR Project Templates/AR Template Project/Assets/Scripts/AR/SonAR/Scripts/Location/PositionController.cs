using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SonAR.Location
{
    public class PositionController : MonoBehaviour
    {
        private static PositionController instance;
        public static PositionController Instance
        {
            get
            {
                if (!instance)
                    instance = FindObjectOfType<PositionController>();
                return instance;
            }
        }

        [SerializeField]
        private AbstractMap mapboxMap;
        public AbstractMap MapboxMap
        {
            get
            {
                return mapboxMap;
            }
        }

        /// <summary>
        /// Location service to use for position updates.
        /// </summary>
        [SerializeField]
        private GPSLocationService locationService;

        [SerializeField, Tooltip("Geographic location used as the origin of the map.")]
        private GeoLocationData origin;

        /// <summary>
        /// Geographic location used as the origin of the map.
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

        /// <summary>
        /// Current position in Unity world space.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
            private set
            {
                transform.position = value;
            }
        }

        /// <summary>
        /// Called everytime the position is updated.
        /// </summary>
        public UnityEvent OnPositionUpdated;

        [SerializeField]
        private TMPro.TextMeshProUGUI originDebug;

        [SerializeField]
        private TMPro.TextMeshProUGUI positionDebug;

        private bool initialized = false;
        private bool receivingLocationUpdates = false;

        public void initialize(GeoLocationData origin)
        {
            Origin = origin;
            
            MapboxMap.OnInitialized += handleMapboxInitialized;

            initializeMapboxMap();

            //Debug
            originDebug.text = "[Origin] " + origin.latitude + " " + origin.longitude;
        }

        /// <summary>
        /// Initializes the visual map for the user's location.
        /// </summary>
        private void initializeMapboxMap()
        {
            //Load mapbox map
            Vector2d mapLocation = new Vector2d(Origin.latitude, Origin.longitude);
            int mapZoomLevel = (int)MapboxMap.Zoom; //Use zoom configured in editor
            MapboxMap.Initialize(mapLocation, mapZoomLevel);
        }
        
        private void handleMapboxInitialized()
        {
            //Subscribe to location updates once map is initialized
            locationService.OnLocationUpdate.AddListener(handleLocationUpdate);

            //Unsubscribe from map initalization
            MapboxMap.OnInitialized -= handleMapboxInitialized;

            initialized = true;
            receivingLocationUpdates = true;
        }

        private void handleLocationUpdate(LocationInfo location)
        {
            GeoLocationData locationData = new GeoLocationData(location);
            Position = LocationInterpreter.geoToWorldPosition(locationData, mapboxMap);

            OnPositionUpdated.Invoke();

            //Log position
            positionDebug.text = "[Position] " + Position;
        }

        private void OnEnable()
        {
            if (!receivingLocationUpdates && initialized) //Resubscribe to location updates if already initialized
            {
                locationService.OnLocationUpdate.AddListener(handleLocationUpdate);
                receivingLocationUpdates = true;
            }
        }

        private void OnDisable()
        {
            if (receivingLocationUpdates) //Unsubscribe from location updates
            {
                locationService.OnLocationUpdate.RemoveListener(handleLocationUpdate);
                receivingLocationUpdates = false;
            }
        }
    }
}
