using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ShaderExtensions
{
    public static string[] getPropertiesByType(ShaderUtil.ShaderPropertyType type, Shader shader)
    {
        List<string> properties = new List<string>();

        if (!shader) //Shader is null
            return properties.ToArray();

        int propertyCount = ShaderUtil.GetPropertyCount(shader);

        for (int i = 0; i < propertyCount; i++)
        {
            if (ShaderUtil.GetPropertyType(shader, i) == type)
                properties.Add(ShaderUtil.GetPropertyName(shader, i));
        }

        return properties.ToArray();
    }
}
