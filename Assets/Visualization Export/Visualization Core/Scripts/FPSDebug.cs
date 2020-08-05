using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSDebug : MonoBehaviour
{
    [SerializeField]
    private Text fpsDisplay;

    [SerializeField, Tooltip("How long in seconds between FPS refreshes.")]
    private float refreshRate = 0.25f;

    private IEnumerator calculateFPS()
    {
        while (true)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            fpsDisplay.text = "FPS: " + fps;

            yield return new WaitForSeconds(refreshRate);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(calculateFPS());
    }
}
