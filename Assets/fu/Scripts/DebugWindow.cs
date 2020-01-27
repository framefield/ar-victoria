using UnityEngine;

public class DebugWindow : MonoBehaviour
{
    public TextMesh debugTextMesh;

    // Use this for initialization
    void Start()
    {
       // textMesh = gameObject.GetComponentInChildren<TextMesh>();
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        if (debugTextMesh != null) { 
            if (debugTextMesh.text.Length > 300)
            {
                debugTextMesh.text = message + "\n";
            }
            else
            {
                debugTextMesh.text += message + "\n";
            }
        }
    }
}