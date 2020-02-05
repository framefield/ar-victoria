using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class POITargetCursorReaction : MonoBehaviour, IFocusable {

    public MeshRenderer PoIToEnable;
    public GameObject textToActivate;
    public float shrinkFactor;
    public float maximumShrink = 0.5f; // In percent.
    private Vector3 startupSize;

    public void Start()
    {
        startupSize = transform.localScale;
    }

    public void OnFocusEnter()
    {
        transform.localScale -= new Vector3(shrinkFactor, 0, shrinkFactor);
    }

    public void OnFocusExit()
    {
        if(transform.localScale.x < startupSize.x * maximumShrink)
        {
            PoIToEnable.enabled = true;
            textToActivate.SetActive(true);
            transform.localScale = startupSize;
        }
    }

    /*
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float positionZ = 10.0f;
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, positionZ);
            startX = position.x;
            position = Camera.main.ScreenToWorldPoint(position);
            lastSpawn = Instantiate(cubePrefab, position, transform.rotation) as GameObject;
            startSize = lastSpawn.transform.localScale.z;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 size = lastSpawn.transform.localScale;
            size.x = startSize + (Input.mousePosition.x - startX) * sizingFactor;
            lastSpawn.transform.localScale = size;
        }
    }
    */


}
