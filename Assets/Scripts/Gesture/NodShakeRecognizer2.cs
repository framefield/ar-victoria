using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NodShakeRecognizer2 : MonoBehaviour {

    private Vector3[] angles;
    private int index;
    private Vector3 centerAngle;
    private int sampleLength = 40;

    public event Action NodHandler;
    public event Action HeadshakeHandler;
    public static NodShakeRecognizer2 Current { get; private set; }

    void Awake()
    {
        Current = this;

    }

    // Use this for initialization
    void Start () {
        angles = new Vector3[sampleLength];
        index = 0;
        centerAngle = Camera.main.transform.rotation.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        angles[index] = Camera.main.transform.rotation.eulerAngles;
        //Debug.Log("Index: " + index + " Angle: " + centerAngle.x);
        index++;
        
        if (index == sampleLength)
        {
            CheckMovement();
            ResetGesture();
        }

    }

    void CheckMovement()
    {
        int up = 0, down = 0;
        for(int i = 0; i < sampleLength; i++)
        {
            if(angles[i].x < centerAngle.x - 3f && down < 2)
            {
                down++;
            }else if(angles[i].x > centerAngle.x  && up < 1)
            {
                up++;
            }
        }

        if(up == 1 && down ==2)
        {
            Debug.Log("Yes");
            NodHandler?.Invoke();
        }

    }

    void ResetGesture()
    {
        angles = new Vector3[sampleLength];
        index = 0;
        centerAngle = Camera.main.transform.rotation.eulerAngles;

    }
}
