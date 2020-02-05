using UnityEngine;

public class LayerChanger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private bool _isHidden=false;

    public bool isHidden
    {
        get { return _isHidden; }
        set { if (value == true)
              {
                gameObject.layer = 15;
              }
              else
              {
                gameObject.layer = 0;
              }
              _isHidden = value;
            }
    }

    public void hideObject()
    {
        gameObject.layer = 15;
    }

    public void showObject()
    {
        gameObject.layer = 0;
    }

    

}
