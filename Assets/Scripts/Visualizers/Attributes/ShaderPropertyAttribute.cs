using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderPropertyAttribute : PropertyAttribute
{
    public ShaderPropertyType propertyType { get; set; }
}

[System.Serializable]
public class ShaderPropertyField
{
    public string fieldName;
    public Shader shader;
    public int selectedIndex;
}

public enum ShaderPropertyType
{
    Color = 0,
    Vector = 1,
    Float = 2,
    Range = 3,
    TexEnv = 4
}

