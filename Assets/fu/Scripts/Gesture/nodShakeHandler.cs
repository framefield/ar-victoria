using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodShakeHandler : MonoBehaviour {

    public AudioSource yesSound;
    public AudioSource noSound;

	// Use this for initialization
	void Start () {
        // Register Callback
        NodShakeReconizer.Current.HeadshakeHandler += OnHadShake;
        NodShakeReconizer.Current.NodHandler += OnNod;
    }
	


	// Update is called once per frame
	void Update () {
		
	}


    void OnNod()
    {
        noSound.Play();
    }

    void OnHadShake()
    {
        yesSound.Play();
    }
}
