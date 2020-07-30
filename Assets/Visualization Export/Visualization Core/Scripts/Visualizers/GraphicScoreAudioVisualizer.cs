using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GraphicScoreAudioVisualizer
{
    [HideInInspector]
    public Material material;

    [HideInInspector]
    public Renderer rend;

    [ShaderProperty(ShaderPropertyType.TexEnv),
     Tooltip("Name of the shader texture being set.")]
    public ShaderPropertyField displacementTexField = new ShaderPropertyField
    {
        fieldName = "_DisplacementTexture"
    };

    [SerializeField, Tooltip("Determines how many displacement values are visualized at once.")]
    private int displacementStreamSize = 1024;
    
    public bool controlDisplacementAmount = true;
    
    public float displacementAmountFactor = 1f;

    [ShaderProperty(ShaderPropertyType.Float),
     Tooltip("Amount of displacement.")]
    public ShaderPropertyField displacementAmountField = new ShaderPropertyField
    {
        fieldName = "_DisplacementAmount"
    };
    
    private Queue<float> displacementValues;

    private Texture2D displacementTex;

    public void initialize()
    {
        material = rend.material;

        float[] defaultDisplacementValues = new float[displacementStreamSize];
        defaultDisplacementValues.setValues(0f);

        //Initialize displacement value stream
        displacementValues = new Queue<float>(defaultDisplacementValues);        

        displacementTex = new Texture2D(displacementStreamSize, 1);
    }

    public void visualize(float amplitude)
    {
        updateDisplacementValues(amplitude);
        generateDisplacementTexture();

        if (controlDisplacementAmount)
        {
            material.SetFloat(displacementAmountField.fieldName, displacementAmountFactor);
        }
    }

    private void generateDisplacementTexture()
    {
        Color[] displacementColors = getDisplacementColors();
        int dimensions = displacementColors.Length;

        displacementTex.SetPixels(displacementColors);

        displacementTex.Apply(); //Apply pixel values
        material.SetTexture(displacementTexField.fieldName, displacementTex);
    }

    private Color[] getDisplacementColors()
    {
        float[] displacementStream = displacementValues.ToArray();

        Color[] colors = new Color[displacementStream.Length];

        for (int i = 0; i < displacementStream.Length; i++)
        {
            float displacement = displacementStream[i];
            colors[i] = new Color(displacement, displacement, displacement, displacement);
        }

        return colors;
    }

    private void updateDisplacementValues(float amplitude)
    {
        displacementValues.Dequeue(); //Remove old value
        displacementValues.Enqueue(amplitude);
    }
}
