using UnityEngine;

public class WorldOriginRotator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(13,34,45)* Time.deltaTime); 
	}
}
