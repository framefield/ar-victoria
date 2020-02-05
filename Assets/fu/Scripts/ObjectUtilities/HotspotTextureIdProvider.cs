/*
 * @authour Patrick Tobias Fischer 
 * 
 **/

using UnityEngine;
using UnityEngine.EventSystems;

public class HotspotTextureIdProvider : MonoBehaviour {

    public float rayDistance;

    public Color[] colors;
    public string[] texts;

    EventSystem guiInfo = EventSystem.current;

    public GameObject demoTestToggle;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 headPosition = Camera.main.transform.position;
        Vector3 gazeDirection = Camera.main.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(headPosition, gazeDirection, out hit))
        {
            GameObject focused = hit.collider.gameObject;

            if (focused != null)
            {


                Renderer renderer = hit.transform.GetComponent<MeshRenderer>();
                Texture2D texture = renderer.material.mainTexture as Texture2D;
                Vector2 pixelUV = hit.textureCoord;
                pixelUV.x *= texture.width;
                pixelUV.y *= texture.height;
                Vector2 tiling = renderer.material.mainTextureScale;
                Color color = texture.GetPixel(Mathf.FloorToInt(pixelUV.x * tiling.x), Mathf.FloorToInt(pixelUV.y * tiling.y));

                int index = FindIndexFromColor(color);


                if (index == 2)
                {
                   
                    demoTestToggle.SetActive(false);
                } else if(index == 3)
                {
                    demoTestToggle.SetActive(true);
                }
                else {
                    Debug.Log(color);

                }
            }
            
        }

    }

    private int FindIndexFromColor(Color color)
    {
        
        for (int i = 0; i < colors.Length; i++)
        {
            Vector4 lookUpColor = new Vector4(colors[i].r, colors[i].g, colors[i].b, colors[i].a);      // Vector used for floating point inaccuracy compensation
            if (Mathf.Abs((lookUpColor - new Vector4(color.r, color.g, color.b, color.a)).magnitude)<0.001f)
            {
                return i;
            }
        }
        return -1;
    }


}
