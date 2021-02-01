using SonAR.Location;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace SonAR.SonicMapping
{
    [CustomEditor(typeof(GeoAudioController))]
    public class GeoAudioControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Only perform updates when not in play mode and not editing a prefab
            if (!Application.isPlaying && PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                GeoAudioController controller = (GeoAudioController)target;

                refreshGeoAudio(controller);
            }
        }

        private void refreshGeoAudio(GeoAudioController ctrllr)
        {
            refreshAudioPosition(ctrllr);
            refreshVisualizerScale(ctrllr);
        }

        private void refreshAudioPosition(GeoAudioController ctrllr)
        {
            //GeoAudio position changed, update location
            if (ctrllr.worldPos != ctrllr.transform.position)
            {
                Vector3 rawWorldPos = ctrllr.transform.position;

                //Constrain location to 0 on y-axis
                Vector3 flatWorldPos = new Vector3(rawWorldPos.x, 0, rawWorldPos.z);
                ctrllr.transform.position = flatWorldPos;
                ctrllr.worldPos = flatWorldPos;

                //Set geo location based on position in Unity world space
                GeoLocationData newLocation =
                    LocationInterpreter.worldToGeoLocation(flatWorldPos, ctrllr.mapboxMap);
                
                ctrllr.geoAudio.Location = newLocation;
            }
        }

        private void refreshVisualizerScale(GeoAudioController ctrllr)
        {
            if (ctrllr.geoAudio.Type == GeoAudio.GeoAudioType.Dynamic)
            {
                //Multiply maxDistance by 2 to set the radius instead of diameter
                ctrllr.visualizer.localScale = ctrllr.audioSource.maxDistance * 2 * Vector3.one;
            }
        }
    }
}