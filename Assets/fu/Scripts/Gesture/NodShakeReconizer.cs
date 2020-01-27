using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.Linq;

public struct PoseSample
{
    public float timestamp;
    public Quaternion orientation;
    public Vector3 eulerAngles;

    public PoseSample(float timestamp, Quaternion orientation)
    {
        this.timestamp = timestamp;
        this.orientation = orientation;

        eulerAngles = orientation.eulerAngles;
        eulerAngles.x = NodShakeReconizer.WrapDegree(eulerAngles.x);
        eulerAngles.y = NodShakeReconizer.WrapDegree(eulerAngles.y);
    }
}


public class NodShakeReconizer : MonoBehaviour {

    public static NodShakeReconizer Current { get; private set; }

    public float recognitionInterval = 0.5f;

    public event Action NodHandler;
    public event Action HeadshakeHandler;

    public Queue<PoseSample> PoseSamples { get; } = new Queue<PoseSample>();

    float prevGestureTime;

    void Awake()
    {
        Current = this;
        
    }


    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        // var orientation = InputTracking.GetLocalRotation(XRNode.Head);
        var orientation = Camera.main.transform.rotation;

        Debug.Log(orientation.eulerAngles.x);
        // Record orientation
        PoseSamples.Enqueue(new PoseSample(Time.time, orientation));
        if (PoseSamples.Count >= 100)
        {
            PoseSamples.Dequeue();
            
        }

        // Recognize gestures
        RecognizeNod();
        RecognizeHeadshake();
    }


    IEnumerable<PoseSample> PoseSamplesWithin(float startTime, float endTime)
    {
        return PoseSamples.Where(sample => sample.timestamp < Time.time - startTime && sample.timestamp >= Time.time - endTime);
    }


    void RecognizeNod()
    {
        try
        {
            var averagePitch = PoseSamplesWithin(0.2f, 0.4f).Average(sample => sample.eulerAngles.x);
            var maxPitch = PoseSamplesWithin(0.01f, 0.2f).Max(sample => sample.eulerAngles.x);
            var minPitch = PoseSamplesWithin(0.01f, 0.2f).Min(sample => sample.eulerAngles.x);
            var pitch = PoseSamples.First().eulerAngles.x;


            if ((maxPitch - averagePitch > 6f && maxPitch - averagePitch < 10f|| averagePitch - minPitch > 6f && averagePitch - minPitch < 10f) &&
                Mathf.Abs(pitch - averagePitch) < 5f)
            {
                if (prevGestureTime < Time.time - recognitionInterval)
                {
                    Debug.Log("NodShakeRecognizer: Nod detected!" );
                    prevGestureTime = Time.time;
                    NodHandler?.Invoke();
                }
            }
        }
        catch (InvalidOperationException)
        {
            // PoseSamplesWithin contains no entry
        }
    }

    void RecognizeHeadshake()
    {
        try
        {
            var averageYaw = PoseSamplesWithin(0.2f, 0.4f).Average(sample => sample.eulerAngles.y);
            var maxYaw = PoseSamplesWithin(0.01f, 0.2f).Max(sample => sample.eulerAngles.y);
            var minYaw = PoseSamplesWithin(0.01f, 0.2f).Min(sample => sample.eulerAngles.y);
            var yaw = PoseSamples.First().eulerAngles.y;

            if ((maxYaw - averageYaw > 10f || averageYaw - minYaw > 10f) &&
                Mathf.Abs(yaw - averageYaw) < 5f)
            {
                if (prevGestureTime < Time.time - recognitionInterval)
                {
                    Debug.Log("NodShakeRecognizer: Shake detected!");
                    prevGestureTime = Time.time;
                    HeadshakeHandler?.Invoke();
                }
            }
        }
        catch (InvalidOperationException)
        {
            // PoseSamplesWithin contains no entry
        }
    }




    // Math helper functions
    public static float LinearMap(float value, float s0, float s1, float d0, float d1)
    {
        return d0 + (value - s0) * (d1 - d0) / (s1 - s0);
    }

    public static float WrapDegree(float degree)
    {
        if (degree > 180f)
        {
            return degree - 360f;
        }
        return degree;
    }
    


}
