using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodShakeHandler3 : MonoBehaviour
{

    public AudioSource yesSound;
    public AudioSource noSound;

    // Use this for initialization
    void Start()
    {
        // Register Callback
        NodShakeRecognizer3.Current.HeadshakeHandler += OnHadShake;
        NodShakeRecognizer3.Current.NodHandler += OnNod;
    }



    // Update is called once per frame
    void Update()
    {

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