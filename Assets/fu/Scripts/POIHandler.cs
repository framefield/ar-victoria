using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class POIHandler : MonoBehaviour, IFocusable {
    public Renderer hitMeshRenderer;
    public Color[] colors;
    public GameObject[] objects;
    
    public Renderer[] POIMeshRenderer;
    private bool isFocused = false;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isFocused)
        {
            GazeManager gazeManager = GazeManager.Instance;
            RaycastHit hitInfo = gazeManager.HitInfo;
            GameObject focused = hitInfo.collider.gameObject;
            if (focused != null)
            {
                //GameObject.Find("Managers").GetComponent<GameObjectManager>().setFocus(focused);
                //Renderer renderer = hitInfo.transform.GetComponent<MeshRenderer>();
               
                hitMeshRenderer.material.SetVector("_GazeUV", new Vector4(hitInfo.textureCoord.x, hitInfo.textureCoord.y, 0.0f, 0.0f));
                Texture2D texture = hitMeshRenderer.material.mainTexture as Texture2D;
             
                Vector2 pixelUV = hitInfo.textureCoord;
                pixelUV.x *= texture.width;
                pixelUV.y *= texture.height;
                Vector2 tiling = hitMeshRenderer.material.mainTextureScale;
                Color color = texture.GetPixel(Mathf.FloorToInt(pixelUV.x * tiling.x), Mathf.FloorToInt(pixelUV.y * tiling.y));
                //Debug.Log(color);
                // Move those two lines to Seperate heatmap script
                //texture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.red);
                //texture.Apply();
                int index = FindIndexFromColor(color);
                if (index >= 0)
                {
                   foreach(GameObject o in objects)
                    {
                        o.SetActive(false);
                    }
                    foreach (Renderer r in POIMeshRenderer)
                    {
                        r.enabled = false;
                     
                    }
                    objects[index].SetActive(true);
                    objects[index].GetComponent<Audio_Trigger>().playAudioAndTrigger();
                    //POIMeshRenderer[index].enabled = true;       
                    
                }
                else
                {
                    /*
                    foreach (GameObject o in objects)
                    {
                        o.SetActive(false);
                    }
                    foreach (Renderer r in POIMeshRenderer)
                    {
                        r.enabled = false;

                    }
                    */
                }
            }
        }
    }

    private int FindIndexFromColor(Color color)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            Vector4 lookUpColor = new Vector4(colors[i].r, colors[i].g, colors[i].b, colors[i].a);
            if (Mathf.Abs((lookUpColor-new Vector4(color.r,color.g,color.b,color.a)).sqrMagnitude)<0.0001f)
            {
                return i;
            }
        }
        return -1;
    }

    public void OnFocusEnter()
    {
        isFocused = true;
    }

    public void OnFocusExit()
    {
        isFocused = false;
    }

    private void changeColour(Color color)
    {
        //Fetch the Renderer from the GameObject
        Renderer rend = GetComponent<Renderer>();

        //Set the main Color of the Material to green
        rend.material.shader = Shader.Find("Custom/POIShader");
        rend.material.SetColor("_ActivePOIColor", color);
      //  Shader.SetGlobalColor("_Color", color);
    }
}
 