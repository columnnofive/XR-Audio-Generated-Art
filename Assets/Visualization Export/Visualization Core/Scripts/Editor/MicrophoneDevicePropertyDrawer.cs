using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioDataSourceController.MicrophoneDevice))]
public class MicrophoneDevicePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty deviceNameProp = property.FindPropertyRelative("name");

        SerializedProperty selectedIndexProp = property.FindPropertyRelative("selectedIndex");
        int selectedIndex = selectedIndexProp.intValue;

        string[] availableDevices = Microphone.devices;

        //Constrain selected to available microphone devices
        if (selectedIndex < 0)
            selectedIndex = 0;
        else if (selectedIndex > availableDevices.Length - 1)
            selectedIndex = availableDevices.Length - 1;

        position = EditorGUI.PrefixLabel(position, label);

        selectedIndex = EditorGUI.Popup(position, selectedIndex, availableDevices);
        selectedIndexProp.intValue = selectedIndex;

        string deviceName = availableDevices.Length > 0 ? availableDevices[selectedIndex] : "";
        deviceNameProp.stringValue = deviceName;

        EditorGUI.EndProperty();
    }
}
