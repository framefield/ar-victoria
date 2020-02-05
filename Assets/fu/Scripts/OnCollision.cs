using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class OnCollision : MonoBehaviour, IFocusable{

    public GameObject cursor;

    public void OnFocusEnter()
    {
        Debug.Log("Enter called");
        GetComponent<Rigidbody>().AddForce(-transform.forward * 2, ForceMode.Impulse);
        GetComponent<Rigidbody>().useGravity = true;
    }

    public void OnFocusExit()
    {
        Debug.Log("Exit called");
    }
}
