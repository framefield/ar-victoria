using System.Collections;
using UnityEngine;

public class Audio_Trigger : MonoBehaviour {

    private AudioSource myAudio;
    private float duration;
    public GameObject myCollider;
    
     

	// Use this for initialization
	void Start () {
        myAudio = GetComponent<AudioSource>();
        duration = myAudio.clip.length;
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void playAudioAndTrigger()
    {
        StartCoroutine(waitASecond());
    }

    IEnumerator waitASecond()
    {
        
        myAudio.Play();
        yield return new WaitForSeconds(duration);
        myCollider.SetActive(true);

        

    }



}
