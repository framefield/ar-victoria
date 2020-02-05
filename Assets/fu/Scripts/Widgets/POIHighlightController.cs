using System.Collections;
using UnityEngine;

public class POIHighlightController : MonoBehaviour {

    public float highlightDuration = 0.5f;
    public int minDelay = 3;
    public int maxDelay = 10;
    public AnimationCurve delayLikeliehood;
    public MeshRenderer[] meshRenderer;

    private bool playAnim = true;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {

        if (playAnim)
            StartCoroutine(WaitAnim()); //wait random seconds for animation
        

    }
    

    public IEnumerator WaitAnim()
    {
        playAnim = false;
        int randomWait = Random.Range(minDelay, maxDelay);
        print("Time" + randomWait + " Play"); //debug                
        yield return new WaitForSeconds(randomWait);
        int itemToSwitch = Random.Range(0, meshRenderer.Length);
        meshRenderer[itemToSwitch].enabled = true;
        yield return new WaitForSeconds(highlightDuration);
        meshRenderer[itemToSwitch].enabled = false;
        playAnim = true;
    }



}
