using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobileDebugger : MonoBehaviour
{
    [SerializeField]
    private Text debugText;

    private void Start()
    {
        debugText.text = ""; //Clear placeholder text
    }

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += handleLogMessage;
    }

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= handleLogMessage;
    }

    private void handleLogMessage(string message, string stackTrace, LogType type)
    {
        debugText.text += message + "\n";
    }
}
