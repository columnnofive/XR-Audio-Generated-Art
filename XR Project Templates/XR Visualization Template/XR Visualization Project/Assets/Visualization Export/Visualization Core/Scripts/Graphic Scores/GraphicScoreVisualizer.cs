using UnityEngine;

public class GraphicScoreVisualizer : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Material material;

    [SerializeField, HideInInspector]
    private Renderer rend;

    [SerializeField]
    private Texture2D graphicScore;

    [SerializeField,
     ShaderProperty(ShaderPropertyType.TexEnv),
     Tooltip("Name of the shader texture being set.")]
    private ShaderPropertyField displacementTexField = new ShaderPropertyField
    {
        fieldName = "_DisplacementTexture"
    };

    [SerializeField]
    private bool controlDisplacementAmount = true;

    [SerializeField]
    private float displacementAmountFactor = 1f;

    [SerializeField,
     ShaderProperty(ShaderPropertyType.Float),
     Tooltip("Amount of displacement.")]
    private ShaderPropertyField displacementAmountField = new ShaderPropertyField
    {
        fieldName = "_DisplacementAmount"
    };

    [SerializeField, ReadOnlyField]
    private float[] displacementValues;
    
    private Texture2D displacementTex;

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
        initialize();
    }

    private void initialize()
    {
        material = rend.material;
        displacementValues = GraphicScoreInterpreter.getDisplacementValues(graphicScore);

        generateDisplacementTexture();
    }

    private void generateDisplacementTexture()
    {
        Color[] displacementColors = getDisplacementColors();
        int dimensions = displacementColors.Length;
        displacementTex = new Texture2D(dimensions, 1);

        displacementTex.SetPixels(displacementColors);

        displacementTex.Apply(); //Apply pixel values
        material.SetTexture(displacementTexField.fieldName, displacementTex);
    }

    private Color[] getDisplacementColors()
    {
        Color[] colors = new Color[displacementValues.Length];

        for (int i = 0; i < displacementValues.Length; i++)
        {
            float displacement = displacementValues[i];
            colors[i] = new Color(displacement, displacement, displacement, displacement);
        }

        return colors;
    }

    private void Update()
    {
        if (controlDisplacementAmount)
        {
            material.SetFloat(displacementAmountField.fieldName, displacementAmountFactor);
        }
    }
}
