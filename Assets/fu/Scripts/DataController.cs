using UnityEngine;


public class DataController : MonoBehaviour {

    public ExhibitionData exhibitionData;
    //public ExhibitionItemData[] allExhibitionItemData;
    public GameObject exhibitionItem;

    private void Start()
    {
       // foreach (ExhibitionItemData data in allExhibitionItemData)
        //{
            LoadItemData(exhibitionItem);
        //}
    }

    private void Update()
    {
        
    }

    private void LoadItemData(GameObject gameObject)
    {
        // Search for Game object and load the data
        gameObject.transform.position = new Vector3(PlayerPrefs.GetFloat("posX"), PlayerPrefs.GetFloat("posY"), PlayerPrefs.GetFloat("posZ"));
        gameObject.transform.eulerAngles = new Vector3(PlayerPrefs.GetFloat("rotX"), PlayerPrefs.GetFloat("rotY"), PlayerPrefs.GetFloat("rotZ"));
        gameObject.transform.localScale = new Vector3(PlayerPrefs.GetFloat("scaleX"), PlayerPrefs.GetFloat("scaleY"), PlayerPrefs.GetFloat("scaleZ"));
    }

    public void SavePosition(GameObject gameObject)
    {
        PlayerPrefs.SetFloat("posX", gameObject.transform.position.x);
        PlayerPrefs.SetFloat("posY", gameObject.transform.position.y);
        PlayerPrefs.SetFloat("posZ", gameObject.transform.position.z);
        PlayerPrefs.SetFloat("rotX", gameObject.transform.eulerAngles.x);
        PlayerPrefs.SetFloat("rotY", gameObject.transform.eulerAngles.y);
        PlayerPrefs.SetFloat("rotZ", gameObject.transform.eulerAngles.z);
        PlayerPrefs.SetFloat("scaleX", gameObject.GetComponent<GameObject>().transform.localScale.x);
        PlayerPrefs.SetFloat("scaleY", gameObject.GetComponent<GameObject>().transform.localScale.y);
        PlayerPrefs.SetFloat("scaleZ", gameObject.GetComponent<GameObject>().transform.localScale.z);
        Debug.Log(PlayerPrefs.GetFloat("scaleX"));
    }

}
