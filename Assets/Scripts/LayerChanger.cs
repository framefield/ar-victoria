using System.Collections;
using System.Collections.Generic;
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
                this.gameObject.layer = 15;
              }
              else
              {
                this.gameObject.layer = 0;
              }
              _isHidden = value;
            }
    }

    public void hideObject()
    {
        this.gameObject.layer = 15;
    }

    public void showObject()
    {
        this.gameObject.layer = 0;
    }

    

}
