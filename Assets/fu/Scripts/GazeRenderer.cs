using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class GazeRenderer : MonoBehaviour
{
    public Renderer hitMeshRenderer;

    void Start()
    {
       
    }

    void Update()
    {

        GazeManager gazeManager = GazeManager.Instance;
        RaycastHit hit = gazeManager.HitInfo;
        GameObject focused = hit.collider.gameObject;
         if (!focused)
          return;


        hitMeshRenderer.material.SetVector("_GazeUV", new Vector4(hit.textureCoord.x, hit.textureCoord.y, 0.0f, 0.0f));
        Texture2D texture = hitMeshRenderer.material.mainTexture as Texture2D;

        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= texture.width;
        pixelUV.y *= texture.height;


        texture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.red);
        texture.Apply();

        // Move those two lines to Seperate heatmap script
        //texture.SetPixel((int)pixelUV.x, (int)pixelUV.y, Color.red);
        //texture.Apply();
    }
}