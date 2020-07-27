using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioDataSourceController))]
public class AudioDataSourceControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Only allow settings to be adjusted when not in playmode
        bool allowEdits = !EditorApplication.isPlaying && !EditorApplication.isPaused;

        if (!allowEdits) //Disable
            GUI.enabled = false;

        AudioDataSourceController controller = (AudioDataSourceController)target;

        SerializedProperty audioDataSourceProp = serializedObject.FindProperty("audioDataSource");
        EditorGUILayout.PropertyField(audioDataSourceProp);

        var audioDataSource = (AudioDataSourceController.AudioDataSource)audioDataSourceProp.enumValueIndex;
        if (audioDataSource == AudioDataSourceController.AudioDataSource.Microphone)
        {
            SerializedProperty micDeviceProp = serializedObject.FindProperty("micDevice");
            EditorGUILayout.PropertyField(micDeviceProp);
        }

        if (!allowEdits) //reenable gui
            GUI.enabled = true;
        else //Save any changes made to serialized properties
            serializedObject.ApplyModifiedProperties();
    }
}
