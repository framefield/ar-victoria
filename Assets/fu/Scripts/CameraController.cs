/*
 *  @authour Fischer
 *
 **/

using UnityEngine;

public class CameraController : MonoBehaviour {

    private Vector3 movement;
    public float speed;

    // Use this for initialization
    void Start () {
		
	}


    private void LateUpdate()
    {
        transform.position += movement * speed;
    }

    // Update is called once per frame
    void FixedUpdate () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        movement = new Vector3(moveHorizontal, 0.0f, moveVertical);


	}
}
