using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicScoreStream : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Material material;

    [SerializeField, HideInInspector]
    private Renderer rend;

    [SerializeField]
    private Texture2D[] graphicScores;

    [SerializeField,
     ShaderProperty(ShaderPropertyType.TexEnv),
     Tooltip("Name of the shader texture being set.")]
    private ShaderPropertyField displacementTexField = new ShaderPropertyField
    {
        fieldName = "_DisplacementTexture"
    };

    [Tooltip("Determines how many displacement values are visualized at once.")]
    public int displacementStreamSize = 1024;
    
    public bool controlDisplacementAmount = true;
    
    public float displacementAmountFactor = 1f;

    [SerializeField,
     ShaderProperty(ShaderPropertyType.Float),
     Tooltip("Amount of displacement.")]
    private ShaderPropertyField displacementAmountField = new ShaderPropertyField
    {
        fieldName = "_DisplacementAmount"
    };

    [SerializeField, ReadOnlyField]
    private float[] displacementValues;
    
    private Queue<float> displacementValueStream;

    private int streamIndex = 0;

    private Texture2D displacementTex;

    private bool initialized = false;

    private void OnValidate()
    {
        if (!rend || rend.transform != transform)
        {
            rend = GetComponent<Renderer>();
        }

        if (rend && rend.sharedMaterial)
        {
            if (rend.sharedMaterial != material) //Material changed
            {
                material = rend.sharedMaterial;
                displacementTexField.shader = material.shader;
                displacementAmountField.shader = material.shader;
            }
        }
        else
        {
            material = null;
            displacementTexField.shader = null;
            displacementAmountField.shader = null;
        }
    }

    private void Start()
    {
        StartCoroutine(initialize());
    }

    private IEnumerator initialize()
    {
        initialized = false;

        material = rend.material;

        List<float> graphicScoreDisplacementValues = new List<float>();
        foreach (Texture2D graphicScore in graphicScores)
        {
            graphicScoreDisplacementValues.AddRange(GraphicScoreInterpreter.getDisplacementValues(graphicScore));
        }        
        displacementValues = graphicScoreDisplacementValues.ToArray();

        //Initialize displacement value stream
        displacementValueStream = new Queue<float>();
        for (int i = 0; i < displacementStreamSize; i++)
            displacementValueStream.Enqueue(displacementValues[i % displacementValues.Length]);

        displacementTex = new Texture2D(displacementStreamSize, 1);

        initialized = true;

        yield return null;
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
        float[] displacementStream = displacementValueStream.ToArray();

        Color[] colors = new Color[displacementStream.Length];

        for (int i = 0; i < displacementStream.Length; i++)
        {
            float displacement = displacementStream[i];
            colors[i] = new Color(displacement, displacement, displacement, displacement);
        }

        return colors;
    }

    private void updateDisplacementValueStream()
    {
        //Stream size didn't change
        if (displacementValueStream.Count == displacementStreamSize)
        {
            displacementValueStream.Dequeue(); //Remove old value
        }
        else if (displacementValueStream.Count - 1 > displacementStreamSize) //Decreased stream size
        {
            if (displacementStreamSize < 0) //Constrain to positive values
                displacementStreamSize = 0;

            //Remove extra values
            while (displacementValueStream.Count - 1 > displacementStreamSize)
                displacementValueStream.Dequeue();

            //Resize texture to match stream size
            displacementTex.Resize(displacementStreamSize, 1);
            displacementTex.Apply(); //Apply changes         
        }

        displacementValueStream.Enqueue(displacementValues[streamIndex]);
        streamIndex++;
        streamIndex = streamIndex % displacementValues.Length; //Constrain index to displacement values

        if (displacementValueStream.Count < displacementStreamSize) //Increased stream size
        {
            //Resize texture to match number of displacement values
            displacementTex.Resize(displacementValueStream.Count, 1);
            displacementTex.Apply(); //Apply changes
        }
    }

    private void Update()
    {
        if (initialized)
        {
            updateDisplacementValueStream();
            generateDisplacementTexture();

            if (controlDisplacementAmount)
            {
                material.SetFloat(displacementAmountField.fieldName, displacementAmountFactor);
            }
        }
    }
}
