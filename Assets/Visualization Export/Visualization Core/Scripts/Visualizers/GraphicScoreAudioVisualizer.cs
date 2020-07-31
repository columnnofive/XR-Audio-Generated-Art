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

    [Tooltip("Determines how many displacement values are visualized at once.")]
    public int displacementStreamSize = 1024;
    
    public bool controlDisplacementAmount = true;
    
    public float displacementAmountFactor = 0.25f;

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
        //Stream size didn't change
        if (displacementValues.Count == displacementStreamSize)
        {
            displacementValues.Dequeue(); //Remove old value
        }
        else if (displacementValues.Count - 1 > displacementStreamSize) //Decreased stream size
        {
            if (displacementStreamSize < 0) //Constrain to positive values
                displacementStreamSize = 0;

            //Remove extra values
            while (displacementValues.Count - 1 > displacementStreamSize)
                displacementValues.Dequeue();

            //Resize texture to match stream size
            displacementTex.Resize(displacementStreamSize, 1);
            displacementTex.Apply(); //Apply changes         
        }

        displacementValues.Enqueue(amplitude);

        if (displacementValues.Count < displacementStreamSize) //Increased stream size
        {
            //Resize texture to match number of displacement values
            displacementTex.Resize(displacementValues.Count, 1);
            displacementTex.Apply(); //Apply changes
        }
    }
}
