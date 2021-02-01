using SonAR.Location;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace SonAR.SonicMapping
{
    [CustomEditor(typeof(GeoLineController))]
    public class GeoLineControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Only perform updates when not in play mode and not editing a prefab
            if (!Application.isPlaying && PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                GeoLineController controller = (GeoLineController)target;

                refreshLinePositions(controller);
            }
        }

        private void refreshLinePositions(GeoLineController ctrllr)
        {
            Vector3[] linePositions = new Vector3[ctrllr.lineRenderer.positionCount];
            ctrllr.lineRenderer.GetPositions(linePositions);

            //Clear old position locations
            ctrllr.geoLine.Positions.Clear();

            for (int i = 0; i < ctrllr.lineRenderer.positionCount; i++)
            {
                Vector3 rawWorldPos = linePositions[i];

                //Constrain location to 0 on y-axis
                Vector3 flatWorldPos = new Vector3(rawWorldPos.x, 0, rawWorldPos.z);
                linePositions[i] = flatWorldPos;

                GeoLocationData location = 
                    LocationInterpreter.worldToGeoLocation(linePositions[i], ctrllr.mapboxMap);

                ctrllr.geoLine.Positions.Add(location);
            }

            //Update line renderer with flattened positions
            ctrllr.lineRenderer.SetPositions(linePositions);
        }
    }
}