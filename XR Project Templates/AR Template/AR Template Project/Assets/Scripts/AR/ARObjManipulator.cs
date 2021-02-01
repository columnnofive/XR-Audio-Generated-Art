using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ARObjManipulator : MonoBehaviour
{
    [Header("Scaling")]

    [SerializeField]
    private float scaleChange = 0.1f;

    [SerializeField]
    private float minScale = 0.001f;

    [SerializeField]
    private InputField scaleInput;
    
    private float scale
    {
        get
        {
            return transform.localScale.x;
        }
        set
        {
            float newScale = value > minScale ? value : minScale;
            transform.localScale = newScale * Vector3.one;
            updateScaleUI();
        }
    }

    private bool isScaling = false;

    private void Start()
    {
        scaleInput.onEndEdit.AddListener(onScaleSet);

        scale = transform.localScale.x;
    }

    public void onScaleSet(string scaleInput)
    {
        if (float.TryParse(scaleInput, out float scaleValue))
            scale = scaleValue;
        else //Scale NaN
            updateScaleUI();
    }

    public void startScaleIncrease()
    {
        StartCoroutine(scaleContinuous(1));
    }

    public void stopScaleIncrease()
    {
        isScaling = false;
    }

    public void startScaleDecrease()
    {
        StartCoroutine(scaleContinuous(-1));
    }

    public void stopScaleDecrease()
    {
        isScaling = false;
    }

    private IEnumerator scaleContinuous(int direction)
    {
        isScaling = true;

        float scaleChange = 0f;

        while (isScaling)
        {
            scaleChange += direction * this.scaleChange * Time.deltaTime;
            scale += scaleChange;

            yield return null;
        }
    }

    private void updateScaleUI()
    {
        scaleInput.text = scale.ToString();
    }
}
