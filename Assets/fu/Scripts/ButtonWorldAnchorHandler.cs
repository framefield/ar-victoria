using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class ButtonWorldAnchorHandler : MonoBehaviour, IFocusable {
    public GameObject ParentGameObjectToPlace;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocusEnter()
    {
        RemoveWorldAnchor();
    }

    public void OnFocusExit()
    {
        AttachWorldAnchor();
    }

    public void translateX(float x)
    {
        ParentGameObjectToPlace.transform.position += new Vector3(x, 0.0f, 0.0f);
    }

    private void AttachWorldAnchor()
    {
        if (WorldAnchorManager.Instance != null)
        {
            // Add world anchor when object placement is done.
            WorldAnchorManager.Instance.AttachAnchor(ParentGameObjectToPlace);
        }
    }

    private void RemoveWorldAnchor()
    {
        if (WorldAnchorManager.Instance != null)
        {
            //Removes existing world anchor if any exist.
            WorldAnchorManager.Instance.RemoveAnchor(ParentGameObjectToPlace);
        }
    }
}
