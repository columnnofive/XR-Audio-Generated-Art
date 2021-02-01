using Supyrb;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VisualizationTriggerAuthoring))]
public class VisualizationTriggerAuthoringPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty triggerTypeProp = property.FindPropertyRelative("triggerType");

        float triggerTypeHeight = EditorGUI.GetPropertyHeight(triggerTypeProp);
        Rect triggerPropPos = new Rect(position.x, position.y, position.width, triggerTypeHeight);
        EditorGUI.PropertyField(triggerPropPos, triggerTypeProp);
        var triggerType = (VisualizationTriggerAuthoring.TriggerType)triggerTypeProp.enumValueIndex;

        SerializedProperty triggerProp = property.FindPropertyRelative("trigger");
        VisualizationTrigger trigger = triggerProp.GetValue<VisualizationTrigger>();

        GUIContent triggerLabel = new GUIContent("Visualization Trigger");

        switch (triggerType)
        {
            case VisualizationTriggerAuthoring.TriggerType.Continuous:
                SerializedProperty continuousTriggerProp = property.FindPropertyRelative("continuousTrigger");

                //Change to continuous trigger
                if (!(trigger is ContinuousTrigger))
                    triggerProp.SetValue<VisualizationTrigger>(continuousTriggerProp.GetValue<ContinuousTrigger>());

                //Rect continuousPos = getPropertyRect(continuousTriggerProp, triggerPropPos);
                //EditorGUI.PropertyField(continuousPos, continuousTriggerProp, triggerLabel, true);
                break;
            case VisualizationTriggerAuthoring.TriggerType.TargetAmplitude:
                SerializedProperty targetAmpTriggerProp = property.FindPropertyRelative("targetAmplitudeTrigger");

                //Change to continuous trigger
                if (!(trigger is TargetAmplitudeTrigger))
                    triggerProp.SetValue<VisualizationTrigger>(targetAmpTriggerProp.GetValue<TargetAmplitudeTrigger>());

                Rect targetAmpPos = getPropertyRect(targetAmpTriggerProp, triggerPropPos);
                EditorGUI.PropertyField(targetAmpPos, targetAmpTriggerProp, triggerLabel, true);
                break;
        }

        property.serializedObject.ApplyModifiedProperties();

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = base.GetPropertyHeight(property, label);
        float additionalHeight = 0f;

        SerializedProperty triggerTypeProp = property.FindPropertyRelative("triggerType");
        var triggerType = (VisualizationTriggerAuthoring.TriggerType)triggerTypeProp.enumValueIndex;

        switch (triggerType)
        {
            case VisualizationTriggerAuthoring.TriggerType.Continuous:
                //SerializedProperty continuousTriggerProp = property.FindPropertyRelative("continuousTrigger");
                //additionalHeight += EditorGUI.GetPropertyHeight(continuousTriggerProp);
                break;
            case VisualizationTriggerAuthoring.TriggerType.TargetAmplitude:
                SerializedProperty targetAmpTriggerProp = property.FindPropertyRelative("targetAmplitudeTrigger");
                additionalHeight += EditorGUI.GetPropertyHeight(targetAmpTriggerProp);
                break;
        }

        return baseHeight + additionalHeight;
    }

    private Rect getPropertyRect(SerializedProperty property, Rect referenceRect)
    {
        float height = EditorGUI.GetPropertyHeight(property);
        return new Rect(referenceRect.x, referenceRect.y + EditorGUIUtility.singleLineHeight, referenceRect.width, height);
    }
}
