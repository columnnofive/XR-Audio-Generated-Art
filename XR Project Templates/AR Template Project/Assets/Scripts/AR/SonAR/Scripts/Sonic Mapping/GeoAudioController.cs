using UnityEngine;
using SonAR.Location;
using Mapbox.Unity.Map;

namespace SonAR.SonicMapping
{
    public class GeoAudioController : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public GeoAudio geoAudio;

        /// <summary>
        /// Last recorded position in world space, used for detecting a position change.
        /// </summary>
        [SerializeField, HideInInspector]
        public Vector3 worldPos;
        
        [SerializeField, HideInInspector]
        public AbstractMap mapboxMap;
        
        [SerializeField, HideInInspector]
        public AudioSource audioSource;
        
        [SerializeField, HideInInspector]
        public Transform visualizer;

        internal void initialize(AbstractMap mapboxMap)
        {
            this.mapboxMap = mapboxMap;

            if (geoAudio.Type == GeoAudio.GeoAudioType.Dynamic)
            {
                audioSource = GetComponent<AudioSource>();

                //Play the audio
                audioSource.Play();
            }

            positionAudio();            
        }

        /// <summary>
        /// Position audio in world space based on geo location.
        /// </summary>
        private void positionAudio()
        {
            transform.position =
                LocationInterpreter.geoToWorldPosition(geoAudio.location, mapboxMap);
        }
    }
}