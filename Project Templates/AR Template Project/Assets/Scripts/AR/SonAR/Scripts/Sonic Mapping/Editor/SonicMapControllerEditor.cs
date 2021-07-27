using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEditor.Experimental.SceneManagement;
using System.Linq;
using SonAR.Location;
using Mapbox.Utils;

namespace SonAR.SonicMapping
{
    [CustomEditor(typeof(SonicMapController))]
    public class SonicMapControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //Only perform updates when not in play mode and not editing a prefab
            if (!Application.isPlaying && PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                SonicMapController controller = (SonicMapController)target;

                refreshSonicMap(controller);
            }
        }

        /// <summary>
        /// Configure SonicMap GameObjects based on current SonicMap settings.
        /// </summary>
        public void refreshSonicMap(SonicMapController ctrllr)
        {
            //Ensure SonicMap has map parent
            if (ctrllr.mapParent == null)
            {
                GameObject mapParent = new GameObject(ctrllr.sonicMap.Name);
                mapParent.tag = SonicMapController.sonicMapTag;
                Undo.RegisterCreatedObjectUndo(mapParent, "Create SonicMap mapParent");
                
                ctrllr.mapParent = mapParent.transform;
            }

            //Update map name
            if (ctrllr.mapParent.name != ctrllr.sonicMap.Name)
                ctrllr.mapParent.name = ctrllr.sonicMap.Name;

            //Update map origin
            Vector2d newCenter = new Vector2d(ctrllr.sonicMap.Origin.latitude, ctrllr.sonicMap.Origin.longitude);
            ctrllr.mapboxMap.SetCenterLatitudeLongitude(newCenter);

            updateGeoAudioObjs(ctrllr);
            updateGeoAnnotationObjs(ctrllr);
            updateGeoLineObjs(ctrllr);


            PrefabUtility.RecordPrefabInstancePropertyModifications(ctrllr);
        }

        private void updateGeoAudioObjs(SonicMapController ctrllr)
        {
            if (ctrllr.geoAudioParent == null)
            {
                GameObject geoAudioParent = new GameObject("Geo Audio Sources");
                ctrllr.geoAudioParent = geoAudioParent.transform;

                ctrllr.geoAudioParent.parent = ctrllr.mapParent;
            }

            if (ctrllr.geoAudioObjMappings == null)
            {
                ctrllr.geoAudioObjMappings = new List<SonicMapController.GeoAudioObjMapping>();
            }

            //Detect new audio sources and update their id's to be unique
            foreach (GeoAudio geoAudio in ctrllr.sonicMap.Audio)
            {
                //Continue incrementing id until there are no duplicates
                while (ctrllr.sonicMap.Audio.Where(audio =>
                    audio.id == geoAudio.id).Count() > 1)
                {
                    geoAudio.id++;
                }
            }

            //Destroy any old GeoAudio GameObjects
            int i = 0;
            while (i < ctrllr.geoAudioObjMappings.Count)
            {
                var mapping = ctrllr.geoAudioObjMappings[i];

                if (mapping.controller == null ||
                    !ctrllr.sonicMap.Audio.Contains(mapping.controller.geoAudio)) //Removed from sonic map
                {
                    //Remove from sonic map
                    ctrllr.geoAudioObjMappings.Remove(mapping);

                    //Remove from scene
                    DestroyImmediate(mapping.gameObject);
                }
                else
                    i++;
            }

            //Add or update GeoAudio GameObjects
            bool addedAudio = false;
            for (int g = 0; g < ctrllr.sonicMap.Audio.Count; g++)
            {
                GeoAudio geoAudio = ctrllr.sonicMap.Audio[g];

                bool exists = false;
                int j = 0;
                while (j < ctrllr.geoAudioObjMappings.Count)
                {
                    var mapping = ctrllr.geoAudioObjMappings[j];

                    //GeoAudio has an instantiated object
                    if (mapping.controller.geoAudio.Equals(geoAudio))
                    {
                        //GeoAudio Name changed
                        if (geoAudio.Name != mapping.gameObject.name)
                            mapping.gameObject.name = geoAudio.Name;

                        exists = true;
                    }
                    j++;
                }

                //GeoAudio doesn't already have a GameObject
                if (!exists)
                {
                    instantiateGeoAudioSource(ctrllr, ref geoAudio);
                    addedAudio = true;
                }
            }

            if (addedAudio)
            {
                //Sort audio by id to ensure they appear in order of creation
                ctrllr.sonicMap.Audio.Sort((a1, a2) => {
                    if (a1.id < a2.id)
                        return -1;
                    else if (a1.id == a2.id)
                        return 0;
                    else // a1.id > a2.id
                        return 1;
                });
            }

            //Remove any child GameObjects from geoAudioParent not mapped to GeoAudio in the SonicMap
            List<Transform> geoAudioChildren = new List<Transform>(ctrllr.geoAudioParent.GetComponentsInChildren<Transform>());
            int k = 0;
            while (k < geoAudioChildren.Count)
            {
                if (geoAudioChildren[k] != null) //Transform may have been destroyed if destroyed object had children
                {
                    GameObject child = geoAudioChildren[k].gameObject;

                    if (child != ctrllr.geoAudioParent.gameObject && //Child is not geoAudioParent
                        child.transform.parent == ctrllr.geoAudioParent && //Child is direct child of geoAudioParent
                        !ctrllr.geoAudioObjMappings.Any(mapping =>
                            mapping.gameObject == geoAudioChildren[k].gameObject)) //Child is not mapped to GeoAudio in SonicMap
                    {
                        DestroyImmediate(geoAudioChildren[k].gameObject); //Remove from heirarchy
                    }
                }

                k++;
            }
        }

        private void updateGeoAnnotationObjs(SonicMapController ctrllr)
        {
            if (ctrllr.geoAnnotationParent == null)
            {
                GameObject geoAnnotationParent = new GameObject("Geo Annotations");
                ctrllr.geoAnnotationParent = geoAnnotationParent.transform;

                ctrllr.geoAnnotationParent.parent = ctrllr.mapParent;
            }

            if (ctrllr.geoAnnotationObjMappings == null)
            {
                ctrllr.geoAnnotationObjMappings = new List<SonicMapController.GeoAnnotationObjMapping>();
            }

            //Detect new annotations and update their id's to be unique
            foreach (GeoAnnotation geoAnnotation in ctrllr.sonicMap.Annotations)
            {
                //Continue incrementing id until there are no duplicates
                while (ctrllr.sonicMap.Annotations.Where(annotation => 
                    annotation.id == geoAnnotation.id).Count() > 1)
                {
                    geoAnnotation.id++;
                }
            }

            //Destroy any old GeoAnnotation GameObjects
            int i = 0;
            while (i < ctrllr.geoAnnotationObjMappings.Count)
            {
                var mapping = ctrllr.geoAnnotationObjMappings[i];
                
                //Removed from sonic map
                if (mapping.controller == null || mapping.textMeshPro == null ||
                    !ctrllr.sonicMap.Annotations.Contains(mapping.controller.geoAnnotation))         
                {
                    //Remove from sonic map
                    ctrllr.geoAnnotationObjMappings.Remove(mapping);

                    //Remove from scene
                    DestroyImmediate(mapping.gameObject);
                }
                else
                    i++;
            }

            //Add or update GeoAnnotation GameObjects
            bool addedAnnotation = false;
            for (int g = 0; g < ctrllr.sonicMap.Annotations.Count; g++)
            {
                GeoAnnotation geoAnnotation = ctrllr.sonicMap.Annotations[g];

                bool exists = false;
                int j = 0;
                while (j < ctrllr.geoAnnotationObjMappings.Count)
                {
                    var mapping = ctrllr.geoAnnotationObjMappings[j];

                    //GeoAnnotation has an instantiated object
                    if (mapping.controller.geoAnnotation.Equals(geoAnnotation))
                    {
                        //GeoAnnotation Text changed
                        if (geoAnnotation.Text != mapping.gameObject.name)
                        {
                            mapping.gameObject.name = geoAnnotation.Text;                            
                            mapping.textMeshPro.text = geoAnnotation.Text;
                        }

                        exists = true;
                    }
                    j++;
                }

                //GeoAnnotation doesn't already have a GameObject
                if (!exists)
                {
                    instantiateGeoAnnotation(ctrllr, ref geoAnnotation);
                    addedAnnotation = true;
                }
            }

            if (addedAnnotation)
            {
                //Sort annotations by id to ensure they appear in order of creation
                ctrllr.sonicMap.Annotations.Sort((a1, a2) => {
                    if (a1.id < a2.id)
                        return -1;
                    else if (a1.id == a2.id)
                        return 0;
                    else // a1.id > a2.id
                        return 1;
                });
            }

            //Remove any child GameObjects from geoAnnotationParent not mapped to GeoAudio in the SonicMap
            List<Transform> geoAnnotationChildren = new List<Transform>(ctrllr.geoAnnotationParent.GetComponentsInChildren<Transform>());
            int k = 0;
            while (k < geoAnnotationChildren.Count)
            {
                if (geoAnnotationChildren[k] != null) //Transform may have been destroyed if destroyed object had children
                {
                    GameObject child = geoAnnotationChildren[k].gameObject;

                    if (child != ctrllr.geoAnnotationParent.gameObject && //Child is not geoAnnotationParent
                        child.transform.parent == ctrllr.geoAnnotationParent && //Child is direct child of geoAnnotationParent
                        !ctrllr.geoAnnotationObjMappings.Any(mapping =>
                            mapping.gameObject == geoAnnotationChildren[k].gameObject)) //Child is not mapped to GeoAnnotations in SonicMap
                    {
                        DestroyImmediate(geoAnnotationChildren[k].gameObject); //Remove from heirarchy
                    }
                }

                k++;
            }
        }

        private void updateGeoLineObjs(SonicMapController ctrllr)
        {
            if (ctrllr.geoLineParent == null)
            {
                GameObject geoLineParent = new GameObject("Geo Line Renderers");
                ctrllr.geoLineParent = geoLineParent.transform;

                ctrllr.geoLineParent.parent = ctrllr.mapParent;
            }

            if (ctrllr.geoLineObjMappings == null)
            {
                ctrllr.geoLineObjMappings = new List<SonicMapController.GeoLineObjMapping>();
            }

            //Detect new lines and update their id's to be unique
            foreach (GeoLine geoLine in ctrllr.sonicMap.Lines)
            {
                //Continue incrementing id until there are no duplicates
                while (ctrllr.sonicMap.Lines.Where(line =>
                    line.id == geoLine.id).Count() > 1)
                {
                    geoLine.id++;
                }
            }

            //Destroy any old GeoLine GameObjects
            int i = 0;
            while (i < ctrllr.geoLineObjMappings.Count)
            {
                var mapping = ctrllr.geoLineObjMappings[i];

                if (mapping.controller == null ||
                    !ctrllr.sonicMap.Lines.Contains(mapping.controller.geoLine)) //Removed from sonic map
                {
                    //Remove from sonic map
                    ctrllr.geoLineObjMappings.Remove(mapping);

                    //Remove from scene
                    DestroyImmediate(mapping.gameObject);
                }
                else
                    i++;
            }

            //Add or update GeoLine GameObjects
            bool addedLine = false;
            for (int g = 0; g < ctrllr.sonicMap.Lines.Count; g++)
            {
                GeoLine geoLine = ctrllr.sonicMap.Lines[g];

                bool exists = false;
                int j = 0;
                while (j < ctrllr.geoLineObjMappings.Count)
                {
                    var mapping = ctrllr.geoLineObjMappings[j];

                    //GeoLine has an instantiated object
                    if (mapping.controller.geoLine.Equals(geoLine))
                    {
                        //GeoLine Name changed
                        if (geoLine.Name != mapping.gameObject.name)
                            mapping.gameObject.name = geoLine.Name;

                        exists = true;
                    }
                    j++;
                }

                //GeoLine doesn't already have a GameObject
                if (!exists)
                {
                    instantiateGeoLine(ctrllr, ref geoLine);
                    addedLine = true;
                }
            }

            if (addedLine)
            {
                //Sort lines by id to ensure they appear in order of creation
                ctrllr.sonicMap.Lines.Sort((l1, l2) => {
                    if (l1.id < l2.id)
                        return -1;
                    else if (l1.id == l2.id)
                        return 0;
                    else // a1.id > a2.id
                        return 1;
                });
            }

            //Remove any child GameObjects from geoLineParent not mapped to GeoLine in the SonicMap
            List<Transform> geoLineChildren = new List<Transform>(ctrllr.geoLineParent.GetComponentsInChildren<Transform>());
            int k = 0;
            while (k < geoLineChildren.Count)
            {
                if (geoLineChildren[k] != null) //Transform may have been destroyed if destroyed object had children
                {
                    GameObject child = geoLineChildren[k].gameObject;

                    if (child != ctrllr.geoLineParent.gameObject && //Child is not geoLineParent
                        child.transform.parent == ctrllr.geoLineParent && //Child is direct child of geoLineParent
                        !ctrllr.geoLineObjMappings.Any(mapping =>
                            mapping.gameObject == geoLineChildren[k].gameObject)) //Child is not mapped to GeoLine in SonicMap
                    {
                        DestroyImmediate(geoLineChildren[k].gameObject); //Remove from heirarchy
                    }
                }

                k++;
            }
        }

        private void instantiateGeoAudioSource(SonicMapController ctrllr, ref GeoAudio geoAudio)
        {
            GameObject audioObj = new GameObject(geoAudio.Name);
            audioObj.transform.parent = ctrllr.geoAudioParent; //Make a child of the geo audio parent

            //Assign default values
            geoAudio.Name = "";
            geoAudio.Location = ctrllr.sonicMap.Origin; //Default location to map origin

            AudioSource audioSource = null;
            if (geoAudio.Type == GeoAudio.GeoAudioType.Dynamic)
            {
                audioSource = audioObj.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1; //Set to 3D spatial audio
                audioSource.maxDistance = 50; //Default maxDistance
            }

            //Instantiate audio visualizer prefab as child of audioObj
            GameObject audioVisualizer = Instantiate(ctrllr.audioVisualizerPrefab);
            audioVisualizer.name = "Audio Visualizer";
            audioVisualizer.transform.parent = audioObj.transform;
            
            //Setup GeoAudioController
            GeoAudioController geoAudioController = audioObj.AddComponent<GeoAudioController>();
            geoAudioController.geoAudio = geoAudio;
            geoAudioController.visualizer = audioVisualizer.transform;
            geoAudioController.mapboxMap = ctrllr.mapboxMap;

            if (geoAudio.Type == GeoAudio.GeoAudioType.Dynamic)
            {
                geoAudioController.audioSource = audioSource;

                //Multiply maxDistance by 2 to set the radius instead of diameter
                audioVisualizer.transform.localScale = audioSource.maxDistance * 2 * Vector3.one;
            }

            ctrllr.geoAudioObjMappings.Add(
                new SonicMapController.GeoAudioObjMapping
                {
                    gameObject = audioObj,
                    controller = geoAudioController,
                    audioSource = audioSource,
                    visualizer = audioVisualizer.transform
                });
        }

        private void instantiateGeoAnnotation(SonicMapController ctrllr, ref GeoAnnotation geoAnnotation)
        {
            //Instantiate based on preconfigured TextMeshPro prefab
            TextMeshPro textMeshPro = Instantiate(ctrllr.annotationPrefab);
            textMeshPro.text = geoAnnotation.Text;

            GameObject annotationObj = textMeshPro.gameObject;
            annotationObj.name = geoAnnotation.Text;
            annotationObj.transform.parent = ctrllr.geoAnnotationParent; //Make a child of the geo annotation parent

            //Assign default values
            geoAnnotation.Text = "";
            geoAnnotation.Location = ctrllr.sonicMap.Origin; //Default location to map origin

            //Setup GeoAnnotationController
            GeoAnnotationController geoAnnotationController = annotationObj.AddComponent<GeoAnnotationController>();
            geoAnnotationController.geoAnnotation = geoAnnotation;
            geoAnnotationController.annotation = textMeshPro;
            geoAnnotationController.mapboxMap = ctrllr.mapboxMap;

            ctrllr.geoAnnotationObjMappings.Add(
                new SonicMapController.GeoAnnotationObjMapping
                {
                    gameObject = annotationObj,
                    controller = geoAnnotationController,
                    textMeshPro = textMeshPro
                });
        }

        private void instantiateGeoLine(SonicMapController ctrllr, ref GeoLine geoLine)
        {
            //Instantiate based on preconfigured LineRenderer prefab
            LineRenderer lineRenderer = Instantiate(ctrllr.lineRendererPrefab);

            GameObject lineObj = lineRenderer.gameObject;
            lineObj.name = geoLine.Name;
            lineObj.transform.parent = ctrllr.geoLineParent; //Make a child of the geo line renderers parent

            //Assign default values
            geoLine.Name = "";

            //Setup GeoLineController
            GeoLineController geoLineController = lineObj.AddComponent<GeoLineController>();
            geoLineController.geoLine = geoLine;
            geoLineController.lineRenderer = lineRenderer;
            geoLineController.mapboxMap = ctrllr.mapboxMap;

            ctrllr.geoLineObjMappings.Add(
                new SonicMapController.GeoLineObjMapping
                {
                    gameObject = lineObj,
                    controller = geoLineController
                });
        }
    }
}