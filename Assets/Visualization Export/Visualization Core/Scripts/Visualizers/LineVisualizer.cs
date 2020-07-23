using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LineVisualizer
{
    [HideInInspector]
    public TrailRenderer trailRenderer;

    [Header("Width")]
    
    public bool controlWidth = true;

    [SerializeField]
    private float minWidth = 0.5f;

    [SerializeField]
    private float maxWidth = 1f;

    [Header("Color")]
    
    public bool controlColor = true;

    [SerializeField, Range(0, 1)]
    private float colorChangeThreshold = 0.5f;

    [SerializeField]
    private float colorChangeSpeed = 1f;

    [SerializeField, Range(0, 1)]
    private float hueMin = 0f;

    [SerializeField, Range(0, 1)]
    private float hueMax = 1f;

    [SerializeField]
    private float emissionIntensity = 1f;

    [ShaderProperty(ShaderPropertyType.Color)]
    public ShaderPropertyField shaderColorField = new ShaderPropertyField
    {
        fieldName = "_EmissionColor"
    };

    private Color targetColor;

    public void setWidth(float bandAmplitude, float visualizerScaleFactor)
    {
        float widthInterpolation = (maxWidth - minWidth) * visualizerScaleFactor * bandAmplitude;
        trailRenderer.widthMultiplier = minWidth + widthInterpolation;
    }

    //Randomly set emission color based on audio band amplitude.
    public void setColor(float bandAmplitude)
    {
        if (bandAmplitude > colorChangeThreshold)
            targetColor = Random.ColorHSV(hueMin, hueMax, 1f, 1f, 0, 1, 1, 1) * emissionIntensity;

        //Lerp to target color
        Color currentColor = trailRenderer.material.GetColor(shaderColorField.fieldName);
        Color color = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorChangeSpeed);

        trailRenderer.material.SetColor(shaderColorField.fieldName, color);
    }
}
