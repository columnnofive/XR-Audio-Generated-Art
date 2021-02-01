using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//Needed for requesting location permissions
#if UNITY_ANDROID
    using UnityEngine.Android;
#endif

#if UNITY_IOS
    using UnityEngine.iOS;
#endif

namespace SonAR.Location
{
    public class GPSLocationService : MonoBehaviour
    {
        [Header("Configuration")]

        [SerializeField, Tooltip("Determines how accurate in meters the GPS location is. Recommends 5-10 for best accuracy.")]
        private float desiredAccuracyInMeters = 5f;

        [SerializeField, Tooltip("Minimum distance in meters the device must move laterally before location is updated.")]
        private float updateDistanceInMeters = 5f;

        [SerializeField, Tooltip("How long in seconds to wait for the location service to initialize before timing out.")]
        private float startupTimeout = 20;

        [SerializeField, Tooltip("How long to wait between location updates.")]
        private float updateDelay = 1f;

        [Header("Location")]

        public UnityEvent OnServiceStart;
        public LocationEvent OnLocationUpdate;
        public UnityEvent OnServiceStop;

        /// <summary>
        /// Last known location.
        /// </summary>
        private LocationInfo lastLocation;

        [Header("Debugging")]

        public TMPro.TextMeshProUGUI locationStatusDebug;
        public TMPro.TextMeshProUGUI locationDebug;

        /// <summary>
        /// Determines if the location service is running.
        /// </summary>
        private bool serviceRunning = false;

        private IEnumerator initializeLocationService()
        {
            locationStatusDebug.text = "[Status] Initializing";
            locationDebug.text = "[Location] N/A";

            if (!Input.location.isEnabledByUser)
            {
                Debug.LogError("Location services are not enabled.");
                locationStatusDebug.text = "[Status] Location services disabled";

                requestLocationPermission();

                //Wait for location permissions
                while (!Input.location.isEnabledByUser)
                {
                    yield return null;
                }
            }

            StartCoroutine(startLocationService());
        }

        private IEnumerator startLocationService()
        {
            locationStatusDebug.text = "[Status] Starting";

            // Start service before querying location
            Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);

            // Wait until service initializes
            float startupTimer = startupTimeout;
            while (Input.location.status == LocationServiceStatus.Initializing && startupTimer > 0)
            {
                yield return null; //Wait until next frame
                startupTimer -= Time.deltaTime;
            }

            // Service didn't initialize within startupTimeout
            if (startupTimer < 1)
            {
                locationStatusDebug.text = "[Status] Timed out";
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                locationStatusDebug.text = "[Status] Unable to determine device location";
                yield break;
            }

            //Connection succeeded, location is ready for use

            OnServiceStart.Invoke();

            serviceRunning = true;
            StartCoroutine(updateLocation());
        }

        private IEnumerator updateLocation()
        {
            locationStatusDebug.text = "[Status] Running";
            int updateCounter = 0;

            while (serviceRunning)
            {
                LocationInfo location = Input.location.lastData;

                //Location changed
                if (location.latitude != lastLocation.latitude &&
                    location.longitude != lastLocation.longitude)
                {
                    lastLocation = location; //Update last location

                    //Notify observers
                    OnLocationUpdate.Invoke(location);

                    locationDebug.text = "[Location] " + location.latitude + " " + location.longitude;
                    locationStatusDebug.text = "[Status] Running, updates: " + updateCounter++;
                }

                yield return new WaitForSeconds(updateDelay);
            }
        }

        private void stopLocationService()
        {
            locationStatusDebug.text = "[Status] Stopped";
            locationDebug.text = "[Location] N/A";

            serviceRunning = false;
            Input.location.Stop();

            OnServiceStop.Invoke();
        }

        private void OnEnable()
        {
            //Need to initialize on enable, coroutines are stopped OnDisable
            StartCoroutine(initializeLocationService());
        }

        private void OnDisable()
        {
            stopLocationService();
        }

        /// <summary>
        /// Requests permission to access location on device.
        /// </summary>
        private static void requestLocationPermission()
        {
#if UNITY_ANDROID
            Permission.RequestUserPermission(Permission.FineLocation);
#endif

#if UNITY_IOS
            //Request location permission in iOS specific style here
#endif
        }
    }

    [System.Serializable]
    public class LocationEvent : UnityEvent<LocationInfo> { }
}