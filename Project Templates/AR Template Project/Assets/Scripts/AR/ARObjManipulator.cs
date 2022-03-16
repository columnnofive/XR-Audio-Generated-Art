using UnityEngine;
using System.Collections;

public class ARObjManipulator : MonoBehaviour
{
    [Header("InitialSettings")]

    [SerializeField]
    private float startScale = 1f;

    [SerializeField]
    private float startHeight = 1f;

    [Header("Scaling")]

    [SerializeField]
    private bool enableScaling = true;

    [SerializeField]
    private float scaleChange = 0.1f;

    [SerializeField]
    private float minScale = 0.001f;

    [SerializeField]
    private float maxScale = 10f;

    private void Start()
    {
        transform.localScale = new Vector3(startScale, startScale, startScale);
        transform.localPosition = new Vector3(0f, startHeight, 0f);
        if (enableScaling)
            StartCoroutine(PinchToScale());
    }

    IEnumerator PinchToScale()
    {
        while (true)
        {
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
                Vector2 touch1PrevPros = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (touch0PrevPos - touch1PrevPros).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                Scale(difference * scaleChange * Time.deltaTime);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    private void Scale(float difference)
    {
        if (transform.localScale.x < 1)
        {
            difference = difference * transform.localScale.x; //difference decreases as the local scale gets close to 0
        }
        transform.localScale += difference * Vector3.one;
        if (transform.localScale.x < minScale) transform.localScale = Vector3.one * minScale;
        else if (transform.localScale.x > maxScale) transform.localScale = Vector3.one * maxScale;
    }

}
