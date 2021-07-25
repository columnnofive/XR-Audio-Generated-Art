using SonAR.Location;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace SonAR.SonicMapping
{
    [CustomEditor(typeof(GeoAnnotationController))]
    public class GeoAnnotationControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Only perform updates when not in play mode and not editing a prefab
            if (!Application.isPlaying && PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                GeoAnnotationController controller = (GeoAnnotationController)target;

                refreshGeoAnnotation(controller);
            }
        }

        private void refreshGeoAnnotation(GeoAnnotationController ctrllr)
        {
            refreshAnnotationPosition(ctrllr);
        }

        private void refreshAnnotationPosition(GeoAnnotationController ctrllr)
        {
            //GeoAnnotation position changed, update location
            if (ctrllr.worldPos != ctrllr.annotation.rectTransform.position)
            {
                Vector3 rawWorldPos = ctrllr.annotation.rectTransform.position;

                //Constrain location to 0 on y-axis
                Vector3 flatWorldPos = new Vector3(rawWorldPos.x, 0, rawWorldPos.z);
                ctrllr.annotation.rectTransform.position = flatWorldPos;
                ctrllr.worldPos = flatWorldPos;

                //Set geo location based on position in Unity world space
                GeoLocationData newLocation =
                    LocationInterpreter.worldToGeoLocation(flatWorldPos, ctrllr.mapboxMap);
                
                ctrllr.geoAnnotation.Location = newLocation;
            }
        }
    }
}