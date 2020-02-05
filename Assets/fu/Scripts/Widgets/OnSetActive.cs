using UnityEngine;

public class OnSetActive : MonoBehaviour {

    public GameObject setThisActive;
    public bool sel;

	// Use this for initialization
	void Start () {
		
	}

    void OnEnable()
    {
        setThisActive.SetActive(sel);
    }


    // Update is called once per frame
    void Update () {
		
	}
}
