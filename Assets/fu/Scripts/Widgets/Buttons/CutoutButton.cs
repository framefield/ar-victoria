using UnityEngine;

public class CutoutButton : MonoBehaviour {

    public float timerDuration = 10f;
    private float lookTimer = 0.001f;

    private Renderer myRenderer;
    private CapsuleCollider myCollider;

    private bool isLookedAt = false;
    public GameObject showObject;

    public AudioSource confirmSound;

    // Use this for initialization
    void Start () {
        
        myCollider = GetComponent<CapsuleCollider>();
        myRenderer = GetComponent<Renderer>();
        myRenderer.material.SetFloat("_Cutoff", 0.001f);
	}
	
	// Update is called once per frame
	void Update () {
        if (isLookedAt)
        {
            lookTimer += Time.deltaTime;
            myRenderer.material.SetFloat("_Cutoff", lookTimer / timerDuration);

            if(lookTimer > timerDuration)
            {
                lookTimer = 0.001f;
                // myCollider.enabled = false;
                isLookedAt = false;
                gameObject.SetActive(false);
                //showObject.SetActive(true);
                confirmSound.Play();
                showObject.GetComponent<Audio_Trigger>().playAudioAndTrigger();
            }
        }
        else
        {
            lookTimer = 0.001f;
            myRenderer.material.SetFloat("_Cutoff", 0.001f);
        }
	}

    public void setGazedAt(bool gazedAt)
    {
        isLookedAt = gazedAt;
    }


    

}
