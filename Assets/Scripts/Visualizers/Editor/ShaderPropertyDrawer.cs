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

        ShaderPropertyAttribute shaderPropertyAttribute = (ShaderPropertyAttribute)attribute;

        var type = convertToPropertyType(shaderPropertyAttribute.propertyType);
        Shader shader = (Shader)property.FindPropertyRelative("shader").objectReferenceValue;
        string[] availableProperties = ShaderExtensions.getPropertiesByType(type, shader);

        SerializedProperty fieldNameProp = property.FindPropertyRelative("fieldName");

        SerializedProperty selectedIndexProp = property.FindPropertyRelative("selectedIndex");
        int selectedIndex = selectedIndexProp.intValue;

        //Constrain selected to properties
        if (selectedIndex < 0)
            selectedIndex = 0;
        else if (selectedIndex > availableProperties.Length - 1)
            selectedIndex = availableProperties.Length - 1;

        position = EditorGUI.PrefixLabel(position, label);

        selectedIndex = EditorGUI.Popup(position, selectedIndex, availableProperties);
        selectedIndexProp.intValue = selectedIndex;

        string fieldName = availableProperties.Length > 0 ? availableProperties[selectedIndex] : "";
        fieldNameProp.stringValue = fieldName;

        EditorGUI.EndProperty();
    }

    private ShaderUtil.ShaderPropertyType convertToPropertyType(ShaderPropertyType type)
    {
        return (ShaderUtil.ShaderPropertyType)type;
    }
}
