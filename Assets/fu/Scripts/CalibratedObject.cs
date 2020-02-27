using UnityEngine;

public class CalibratedObject : MonoBehaviour
{
    void Start () {

        // Check if World Anchor is set. Then load fine calibration from PlayerPrefs. Should change to JSON file Persistance later
#if !UNITY_WSA || UNITY_EDITOR
        Debug.LogWarning("World Anchor Manager does only work on WSA-build. Ignoring saved World Anchor.");
        Debug.Log("Before Loading PlayerPrefs: posX: " + transform.position.x + " posy: " + transform.position.y);
        //LoadPosition();
        
        Debug.Log("After Loading PlayerPrefs: posX: " + transform.position.x + " posy: " + transform.position.y);
        Debug.Log("ScaleX Loaded from PlayerPrefs: " + PlayerPrefs.GetFloat("scaleX"));
#else
        var anchors = FindObjectsOfType<UnityEngine.XR.WSA.WorldAnchor>();
        if (anchors != null)
        {  
            if(PlayerPrefs.HasKey("initialized")){
                Debug.Log("Before Loading PlayerPrefs: posX: " + this.transform.position.x + " posy: " + this.transform.position.y);        
//                LoadPosition();
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

    private void SavePosition()
    {
        PlayerPrefs.SetFloat("posX", transform.localPosition.x);
        PlayerPrefs.SetFloat("posY", transform.localPosition.y);
        PlayerPrefs.SetFloat("posZ", transform.localPosition.z);
        PlayerPrefs.SetFloat("rotX", transform.localEulerAngles.x);
        PlayerPrefs.SetFloat("rotY", transform.localEulerAngles.y);
        PlayerPrefs.SetFloat("rotZ", transform.localEulerAngles.z);
        PlayerPrefs.SetFloat("scaleX", transform.localScale.x);
        PlayerPrefs.SetFloat("scaleY", transform.localScale.y);
        PlayerPrefs.SetFloat("scaleZ", transform.localScale.z);
//        Debug.Log(name + ": " +PlayerPrefs.GetFloat("scaleX") + " scaleX; posX: " + PlayerPrefs.GetFloat("posX") + " rotY: " +
//           PlayerPrefs.GetFloat("rotY"));
    }
    
    public void ResetCalibration()
    {
        var t = transform;
        t.localPosition = new Vector3(0f,0f,0f);
        t.eulerAngles = new Vector3(0f,0f,0f);
        t.localScale = new Vector3(1,1,1);
        SavePosition();
    }
    public void Translate(Vector3 translation)
    {
        transform.localPosition +=translation;
        SavePosition();
    }
    public void RotateY(float y)
    {
        transform.localEulerAngles += (new Vector3(0.0f, y, 0.0f));
        SavePosition();
    }
    
    public void ScaleUniform(float factor)
    {
        
        transform.localScale += new Vector3(factor, factor, factor);
        
        SavePosition();
    }


    public void SetPosition(Vector3 p)
    {
        transform.position = p;
        SavePosition();
    }
}
