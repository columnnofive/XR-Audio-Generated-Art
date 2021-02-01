using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShaderPropertyAttribute))]
public class ShaderPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        ShaderPropertyAttribute shaderPropAttribute = (ShaderPropertyAttribute)attribute;

        ShaderUtil.ShaderPropertyType[] types =
            new ShaderUtil.ShaderPropertyType[shaderPropAttribute.propertyTypes.Length];

        for (int i = 0; i < shaderPropAttribute.propertyTypes.Length; i++)
        {
            types[i] = convertToPropertyType(shaderPropAttribute.propertyTypes[i]);
        }

        Shader shader = (Shader)property.FindPropertyRelative("shader").objectReferenceValue;

        //Get shader properties of the given types
        List<string> availableProperties = new List<string>();
        foreach (ShaderUtil.ShaderPropertyType type in types)
        {
            availableProperties.AddRange(ShaderExtensions.getPropertiesByType(type, shader));
        }

        SerializedProperty fieldNameProp = property.FindPropertyRelative("fieldName");

        SerializedProperty selectedIndexProp = property.FindPropertyRelative("selectedIndex");
        int selectedIndex = selectedIndexProp.intValue;

        //Constrain selected to properties
        if (selectedIndex < 0)
            selectedIndex = 0;
        else if (selectedIndex > availableProperties.Count - 1)
            selectedIndex = availableProperties.Count - 1;

        position = EditorGUI.PrefixLabel(position, label);

        selectedIndex = EditorGUI.Popup(position, selectedIndex, availableProperties.ToArray());
        selectedIndexProp.intValue = selectedIndex;

        string fieldName = availableProperties.Count > 0 ? availableProperties[selectedIndex] : "";
        fieldNameProp.stringValue = fieldName;

        EditorGUI.EndProperty();
    }

    private ShaderUtil.ShaderPropertyType convertToPropertyType(ShaderPropertyType type)
    {
        return (ShaderUtil.ShaderPropertyType)type;
    }
}
