using System;
using System.IO;
using UnityEngine;

public class MovementLogger : MonoBehaviour {
    public float intervall = 1.0f;
    public string filePrefix = "UserMovement";
    private Vector3 referencePos;
    private Vector3 referenceEulerAngles;
    private float startTime = 0.0f;
    private TextWriter writer;
    private bool isLogging = false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        startTime += Time.deltaTime;
        if(startTime>=intervall && isLogging)
        {
            startTime -= intervall;
            writeValues();
        }
	}

    public void StartLogging()
    {
        if(isLogging)
        {
            EndLogging();
        }
        string path = Path.Combine(Application.persistentDataPath, filePrefix + DateTime.Now.ToFileTimeUtc()+ ".csv");
        Debug.LogWarning(path);
        writer = File.CreateText(path);
        referencePos = transform.position;
        referenceEulerAngles = transform.rotation.eulerAngles;
        writeValues();
        isLogging = true;
        startTime = 0;
    }

    public void EndLogging()
    {
        writer.Close();
        isLogging = false;
    }

    private void writeValues()
    {
        writer.Write(transform.position.x);
        writer.Write(",");
        writer.Write(transform.position.y);
        writer.Write(",");
        writer.Write(transform.position.z);
        writer.Write(",");
        writer.Write(transform.rotation.eulerAngles.x);
        writer.Write(",");
        writer.Write(transform.rotation.eulerAngles.y);
        writer.Write(",");
        writer.Write(transform.rotation.eulerAngles.z);
        writer.Write("\r\n");
        writer.Flush();
    }
}
