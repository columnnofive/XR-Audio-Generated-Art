using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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

    [SerializeField, Tooltip("How long to wait in seconds to update the displacement texture.")]
    private float textureUpdateRate = 0f;

    private float displacement = 0f;
    private Texture2D displacementTex;
    private const int displacementTexHeight = 1;
    private int displacementPixelIndex = 0;

    private float textureUpdateTimer = 0f;
    private bool textureNeedsUpdate = false;

    public void OnValidate(GameObject gameObject)
    {
        if (!rend || rend.transform != gameObject.transform)
        {
            rend = gameObject.GetComponent<Renderer>();
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

        constrainDisplacementStreamSize();
    }

    //public void initialize()
    //{
    //    material = rend.material;

    //    float[] defaultDisplacementValues = new float[displacementStreamSize];
    //    defaultDisplacementValues.setValues(0f);

    //    //Initialize black displacement texture
    //    displacementTex = new Texture2D(displacementStreamSize, displacementTexHeight);
    //    Color32[] initialPixels = new Color32[displacementStreamSize * displacementTexHeight];
    //    initialPixels.setValues(getDisplacementColor(0f));
    //    setPixels(displacementTex, initialPixels);

    //    //Set texture in shader
    //    material.SetTexture(displacementTexField.fieldName, displacementTex);
    //}

    //public void visualize(float amplitude)
    //{
    //    constrainDisplacementStreamSize();
    //    updateDisplacementValue(amplitude);
    //    updateDisplacementTexture();

    //    if (controlDisplacementAmount)
    //    {
    //        material.SetFloat(displacementAmountField.fieldName, displacementAmountFactor);
    //    }
    //}

    //private void updateDisplacementTexture()
    //{
    //    Color32 displacementColor = getDisplacementColor(displacement);

    //    setPixel(displacementTex, displacementPixelIndex, displacementColor, false);

    //    if (textureNeedsUpdate)
    //    {
    //        if (textureUpdateTimer > textureUpdateRate)
    //        {
    //            displacementTex.Apply();
    //            textureUpdateTimer = 0f;
    //            textureNeedsUpdate = false;
    //        }
    //        else
    //            textureUpdateTimer += Time.unscaledDeltaTime;
    //    }

    //    //Update displacement value index
    //    displacementPixelIndex = displacementTex.width > 0 ? (displacementPixelIndex + 1) % displacementTex.width : 0;
    //}

    private Color32 getDisplacementColor(float displacement)
    {
        byte displacementByte = (byte)(displacement * 255f);
        return new Color32(displacementByte, displacementByte, displacementByte, displacementByte);
    }

    //private void updateDisplacementValue(float amplitude)
    //{
    //    displacement = amplitude;

    //    //if (displacementTex.width > displacementStreamSize) //Decreased stream size
    //    //{
    //    //    if (displacementStreamSize < 0) //Constrain to positive values
    //    //        displacementStreamSize = 0;

    //    //    //Get current pixels before resizing
    //    //    Color32[] currentPixels = displacementTex.GetPixels32(0, 0, displacementStreamSize, displacementTexHeight);

    //    //    //Resize texture to match stream size
    //    //    displacementTex.Resize(displacementStreamSize, displacementTexHeight);

    //    //    //Maintain current pixels
    //    //    displacementTex.SetPixels32(0, 0, currentPixels.Length, displacementTexHeight, currentPixels);

    //    //    displacementTex.Apply(); //Apply changes
    //    //}
    //    //else if (displacementTex.width < displacementStreamSize) //Increased stream size
    //    //{
    //    //    //Get current pixels before resizing
    //    //    Color32[] currentPixels = displacementTex.GetPixels32();

    //    //    //Resize texture to match stream size
    //    //    displacementTex.Resize(displacementStreamSize, displacementTexHeight);

    //    //    //Maintain current pixels
    //    //    displacementTex.SetPixels32(0, 0, currentPixels.Length, displacementTexHeight, currentPixels);

    //    //    Color32[] fillerPixels = new Color32[displacementStreamSize - currentPixels.Length];
    //    //    fillerPixels.setValues(getDisplacementColor(0f));
    //    //    displacementTex.SetPixels(currentPixels.Length, 0, fillerPixels.Length, displacementTexHeight, fillerPixels);

    //    //    displacementTex.Apply(); //Apply changes
    //    //}
    //}

    private void setPixels(Texture2D tex, Color32[] colors, bool applyChanges = true)
    {
        NativeArray<Color32> textureData = tex.GetRawTextureData<Color32>();

        for (int i = 0; i < textureData.Length && i < colors.Length; i++)
        {
            textureData[i] = colors[i];
        }

        if (applyChanges)
            tex.Apply();
        else
            textureNeedsUpdate = true;
    }

    private void setPixel(Texture2D tex, int x, Color32 color, bool applyChanges = true)
    {
        NativeArray<Color32> textureData = tex.GetRawTextureData<Color32>();

        if (x < textureData.Length)
        {
            textureData[x] = color;

            if (applyChanges)
                tex.Apply();
            else
                textureNeedsUpdate = true;
        }
    }

    private void constrainDisplacementStreamSize()
    {
        //Constrain to positive value
        if (displacementStreamSize < 0)
            displacementStreamSize = 0;

        //Constrain to power of two
        if (!Mathf.IsPowerOfTwo(displacementStreamSize))
            displacementStreamSize = Mathf.ClosestPowerOfTwo(displacementStreamSize);
    }

    //------------------------------------------------------------------------------------------------------------------------------------------------

    private Queue<float> displacementValues;

    public void initialize()
    {
        material = rend.material;

        float[] defaultDisplacementValues = new float[displacementStreamSize];
        defaultDisplacementValues.setValues(0f);

        //Initialize displacement value stream
        displacementValues = new Queue<float>(defaultDisplacementValues);

        //Initialize black displacement texture
        displacementTex = new Texture2D(displacementStreamSize, displacementTexHeight);
        Color32[] initialPixels = new Color32[displacementStreamSize * displacementTexHeight];
        initialPixels.setValues(getDisplacementColor(0f));
        setPixels(displacementTex, initialPixels);

        //Set texture in shader
        material.SetTexture(displacementTexField.fieldName, displacementTex);
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
        Color32[] displacementColors = getDisplacementColors();
        setPixels(displacementTex, displacementColors);
    }

    private Color32[] getDisplacementColors()
    {
        float[] displacementStream = displacementValues.ToArray();

        Color32[] colors = new Color32[displacementStream.Length];

        for (int i = 0; i < displacementStream.Length; i++)
        {
            colors[i] = getDisplacementColor(displacementStream[i]);
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

    //[HideInInspector]
    //public Material material;

    //[HideInInspector]
    //public Renderer rend;

    //[ShaderProperty(ShaderPropertyType.TexEnv),
    // Tooltip("Name of the shader texture being set.")]
    //public ShaderPropertyField displacementTexField = new ShaderPropertyField
    //{
    //    fieldName = "_DisplacementTexture"
    //};

    //[Tooltip("Determines how many displacement values are visualized at once.")]
    //public int displacementStreamSize = 1024;

    //public bool controlDisplacementAmount = true;

    //public float displacementAmountFactor = 0.25f;

    //[ShaderProperty(ShaderPropertyType.Float),
    // Tooltip("Amount of displacement.")]
    //public ShaderPropertyField displacementAmountField = new ShaderPropertyField
    //{
    //    fieldName = "_DisplacementAmount"
    //};

    //private Queue<float> displacementValues;
    //private Texture2D displacementTex;

    //public void initialize()
    //{
    //    material = rend.material;

    //    float[] defaultDisplacementValues = new float[displacementStreamSize];
    //    defaultDisplacementValues.setValues(0f);

    //    //Initialize displacement value stream
    //    displacementValues = new Queue<float>(defaultDisplacementValues);        

    //    displacementTex = new Texture2D(displacementStreamSize, 1);
    //}

    //public void visualize(float amplitude)
    //{
    //    updateDisplacementValues(amplitude);
    //    generateDisplacementTexture();

    //    if (controlDisplacementAmount)
    //    {
    //        material.SetFloat(displacementAmountField.fieldName, displacementAmountFactor);
    //    }
    //}

    //private void generateDisplacementTexture()
    //{
    //    Color[] displacementColors = getDisplacementColors();
    //    int dimensions = displacementColors.Length;

    //    displacementTex.SetPixels(displacementColors);

    //    displacementTex.Apply(); //Apply pixel values
    //    material.SetTexture(displacementTexField.fieldName, displacementTex);
    //}

    //private Color[] getDisplacementColors()
    //{
    //    float[] displacementStream = displacementValues.ToArray();

    //    Color[] colors = new Color[displacementStream.Length];

    //    for (int i = 0; i < displacementStream.Length; i++)
    //    {
    //        float displacement = displacementStream[i];
    //        colors[i] = new Color(displacement, displacement, displacement, displacement);
    //    }

    //    return colors;
    //}

    //private void updateDisplacementValues(float amplitude)
    //{
    //    //Stream size didn't change
    //    if (displacementValues.Count == displacementStreamSize)
    //    {
    //        displacementValues.Dequeue(); //Remove old value
    //    }
    //    else if (displacementValues.Count - 1 > displacementStreamSize) //Decreased stream size
    //    {
    //        if (displacementStreamSize < 0) //Constrain to positive values
    //            displacementStreamSize = 0;

    //        //Remove extra values
    //        while (displacementValues.Count - 1 > displacementStreamSize)
    //            displacementValues.Dequeue();

    //        //Resize texture to match stream size
    //        displacementTex.Resize(displacementStreamSize, 1);
    //        displacementTex.Apply(); //Apply changes         
    //    }

    //    displacementValues.Enqueue(amplitude);

    //    if (displacementValues.Count < displacementStreamSize) //Increased stream size
    //    {
    //        //Resize texture to match number of displacement values
    //        displacementTex.Resize(displacementValues.Count, 1);
    //        displacementTex.Apply(); //Apply changes
    //    }
    //}
}
