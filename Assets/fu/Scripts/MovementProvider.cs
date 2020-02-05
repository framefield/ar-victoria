using System.IO;
using UnityEngine;

public class MovementProvider : MonoBehaviour {
    public float intervall = 1.0f;
    public string filename = "UserMovement132060401998212866.csv";
    
    private Vector3 referencePos;
    private Vector3 referenceEulerAngles;
    private float startTime = 0.0f;
    private StreamReader reader;
    private bool isPlaying = false;


    // Use this for initialization
    void Start () {
		
	}
	


	// Update is called once per frame
	void Update () {
        startTime += Time.deltaTime;
        if (startTime >= intervall && isPlaying)
        {
            startTime -= intervall;
            provideValues();
        }
    }

    public void StartPlayback()
    {
        if (isPlaying)
        {
            EndPlayback();
        }
              
        string fileToLoad = Path.Combine(Application.persistentDataPath, filename);
        reader = new StreamReader(fileToLoad);
        Debug.LogWarning(fileToLoad);
        
        provideValues();
        isPlaying = true;
        startTime = 0;
    }

    public void EndPlayback()
    {

    }

    void provideValues()
    {
        if (reader.EndOfStream) { isPlaying = false; return; }

        string line = reader.ReadLine(); 
        string[] dof = line.Split(',');
        Vector3 rotationVecor = new Vector3(float.Parse(dof[3]), float.Parse(dof[4]), float.Parse(dof[5]));
        Quaternion quat = Quaternion.Euler(rotationVecor);
        transform.SetPositionAndRotation( new Vector3(float.Parse(dof[0]), float.Parse(dof[1]), float.Parse(dof[2])),  quat);
        
      
    }
}
