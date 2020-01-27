using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.XR.WSA;

public class CalibrationHandler : MonoBehaviour
{
   // public GameObject ParentGameObjectToPlace;
   
    // Use this for initialization
    void Start () {
        

        //ResetPosition();
        // Check if World Anchor is set. Then load fine calibration from PlayerPrefs. Should change to JSON file Persistance later
#if !UNITY_WSA || UNITY_EDITOR
        Debug.LogWarning("World Anchor Manager does not work for this build. Ignoring saved World Anchor.");
        Debug.Log("Before Loading PlayerPrefs: posX: " + this.transform.position.x + " posy: " + this.transform.position.y);
        //LoadPosition();
        
        Debug.Log("After Loading PlayerPrefs: posX: " + this.transform.position.x + " posy: " + this.transform.position.y);
        Debug.Log("ScaleX Loaded from PlayerPrefs: " + PlayerPrefs.GetFloat("scaleX"));
#else
        var anchors = FindObjectsOfType<WorldAnchor>();
        if (anchors != null)
        {  
            if(PlayerPrefs.HasKey("initialized")){
                Debug.Log("Before Loading PlayerPrefs: posX: " + this.transform.position.x + " posy: " + this.transform.position.y);        
                LoadPosition();
                Debug.Log("After Loading PlayerPrefs: posX: " + this.transform.position.x + " posy: " + this.transform.position.y);
                Debug.Log("ScaleX Loaded from PlayerPrefs: " + PlayerPrefs.GetFloat("scaleX"));
            } else {
                SavePosition();
                PlayerPrefs.SetInt("initialized", 1);
                Debug.Log("PlayerPrefs Initialized with ItemTransformations.");
            }   
        } else if(anchors == null){
            PlayerPrefs.DeleteAll();
            Debug.Log("All PlayerPrefs were Deleted, because no World Anchor found.");
        }
#endif


    }

    // Update is called once per frame
    void Update () {
		
	}

    public void SavePosition()
    {
        PlayerPrefs.SetFloat("posX", this.transform.localPosition.x);
        PlayerPrefs.SetFloat("posY", this.transform.localPosition.y);
        PlayerPrefs.SetFloat("posZ", this.transform.localPosition.z);
        PlayerPrefs.SetFloat("rotX", this.transform.localEulerAngles.x);
        PlayerPrefs.SetFloat("rotY", this.transform.localEulerAngles.y);
        PlayerPrefs.SetFloat("rotZ", this.transform.localEulerAngles.z);
        PlayerPrefs.SetFloat("scaleX", this.transform.localScale.x);
        PlayerPrefs.SetFloat("scaleY", this.transform.localScale.y);
        PlayerPrefs.SetFloat("scaleZ", this.transform.localScale.z);
        Debug.Log(this.name + ": " +PlayerPrefs.GetFloat("scaleX") + " scaleX; posX: " + PlayerPrefs.GetFloat("posX") + " rotY: " +
           PlayerPrefs.GetFloat("rotY"));
    }


    public void LoadPosition()
    {

        this.transform.localPosition = new Vector3(PlayerPrefs.GetFloat("posX"), PlayerPrefs.GetFloat("posY"), PlayerPrefs.GetFloat("posZ"));
        this.transform.localEulerAngles = new Vector3(PlayerPrefs.GetFloat("rotX"), PlayerPrefs.GetFloat("rotY"), PlayerPrefs.GetFloat("rotZ"));
        this.transform.localScale = new Vector3(PlayerPrefs.GetFloat("scaleX"), PlayerPrefs.GetFloat("scaleY"), PlayerPrefs.GetFloat("scaleZ"));
        
    }

    public void ResetCalibration()
    {
        // PlayerPrefs.DeleteAll();
        this.transform.localPosition = new Vector3(0f,0f,0f);
        this.transform.eulerAngles = new Vector3(0f,0f,0f);
        this.transform.localScale = new Vector3(1,1,1);
        SavePosition();
    }


    public void translateX(float x)
    {
        this.transform.localPosition += new Vector3(x, 0.0f, 0.0f);
        SavePosition();
    }

    public void translateY(float y)
    {
        this.transform.localPosition += new Vector3(0.0f, y, 0.0f);
        SavePosition();
    }

    public void translateZ(float z)
    {
        this.transform.localPosition += new Vector3(0.0f, 0.0f, z);
        SavePosition();
    }

    public void rotateX(float x)
    {
        this.transform.localEulerAngles += (new Vector3(x, 0.0f, 0.0f));
        SavePosition();
    }

    public void rotateY(float y)
    {
        this.transform.localEulerAngles += (new Vector3(0.0f, y, 0.0f));
        SavePosition();
    }

    public void rotateZ(float z)
    {
        this.transform.localEulerAngles += (new Vector3(0.0f, z, 0.0f));
        SavePosition();
    }

    public void scaleUniform(float factor)
    {
        
        this.transform.localScale += new Vector3(factor, factor, factor);
        
        SavePosition();
    }
}
