using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicScorePulser : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Material material;

    [SerializeField, HideInInspector]
    private Renderer rend;

    [SerializeField]
    private Texture2D graphicScore;

    [SerializeField,
     ShaderProperty(ShaderPropertyType.Range, ShaderPropertyType.Float),
     Tooltip("Name of the shader float being set.")]
    private ShaderPropertyField displacementAmountFloat = new ShaderPropertyField
    {
        fieldName = "_DisplacementAmount"
    };

    [SerializeField, Tooltip("How long to spend on each displacement value")]
    private float pulseDelay = 0f;

    [SerializeField]
    private float displacementAmountFactor = 1f;

    [SerializeField, ReadOnlyField]
    private float targetDisplacement = 0f;

    [SerializeField, ReadOnlyField]
    private int index = 0;

    [SerializeField, ReadOnlyField]
    private float[] displacementValues;

    private float pulseTimer = 0f;

    private void OnValidate()
    {
        if (!rend)
        {
            rend = GetComponent<Renderer>();
            if (rend)
                material = rend.sharedMaterial;
            else
                material = null;
        }

        if (rend && rend.sharedMaterial)
        {
            displacementAmountFloat.shader = material.shader;
        }
        else
            displacementAmountFloat.shader = null;
    }

    private void Start()
    {
        initialize();
    }

    private void initialize()
    {
        material = rend.material;
        displacementValues = GraphicScoreInterpreter.getDisplacementValues(graphicScore);
    }
    
    private void Update()
    {
        if (displacementValues.Length > 0)
        {
            float currentDisplacement = material.GetFloat(displacementAmountFloat.fieldName);

            pulseTimer += Time.deltaTime;
            float interpolation = Mathf.Clamp(pulseTimer / pulseDelay, 0f, 1f);
            
            float displacement = Mathf.Lerp(currentDisplacement, targetDisplacement, interpolation);
            material.SetFloat(displacementAmountFloat.fieldName, displacement);

            if (pulseTimer >= pulseDelay)
            {
                if (index < displacementValues.Length - 1)
                {
                    index++;
                    pulseTimer = 0f;
                }
                else
                {
                    index = 0;
                    pulseTimer = 0f;
                }

                targetDisplacement = displacementValues[index] * displacementAmountFactor;
            }
        }
    }
}
