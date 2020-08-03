using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSDebug : MonoBehaviour
{
    [SerializeField]
    private Text fpsDisplay;

    private void Update()
    {
        int fps = (int)(1f / Time.unscaledDeltaTime);
        fpsDisplay.text = "FPS: " + fps;
    }
}
