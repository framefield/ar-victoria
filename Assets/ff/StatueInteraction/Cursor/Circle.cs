#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Circle : MonoBehaviour
{
    [SerializeField]
    public float Radius = 1f;

    [SerializeField]
    public float Width = 0.1f;

    [SerializeField]
    public float StartAngle = 0;

    [SerializeField]
    public float FillAngle = 15;

    [Range(0, 1)]
    [SerializeField]
    public float FillRatio = 0.5f;

    [Range(0, 100)]
    [SerializeField]
    public int Repeat = 1;

    [SerializeField]
    public float Twist;

    [Range(1, 360)]
    [SerializeField]
    public int Resolution = 64;

    [SerializeField]
    public bool BothSided;

    [SerializeField]
    public TextureMode _textureMode;

    [SerializeField]
    public Axis _axis = Axis.Y;


    void Awake()
    {
        InitializeParamsFromScriptProperties();
        _renderer = GetComponent<MeshRenderer>();
        if (!_renderer)
        {
            Debug.LogWarning("" + this + " couldn't find renderer", this);
        }

        _meshFilter = GetComponent<MeshFilter>();
        UpdateMeshIfParamsChanged();
    }

    void Update()
    {
        UpdateMeshIfParamsChanged();
    }

#if UNITY_EDITOR

    [ContextMenu("Bake")]
    private void Bake()
    {
        string filePath = 
            EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "Procedural Mesh", "asset", "");
        if (filePath == "") return;
        AssetDatabase.CreateAsset(_mesh, filePath);  
    }
    
    // Only update when group changes in editor
    private void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && !BuildPipeline.isBuildingPlayer)
        {
            // We cannout call DestroyImmediate from onValidate, thus we defer the destruction
            EditorApplication.delayCall += UpdateAfterOnValidate;
        }
    }
#endif


    private void UpdateAfterOnValidate()
    {
        // NOTE: sadly, this would trigger a rebuilding of the mesh after exiting playmode
        // UpdateMeshIfParamsChanged();
    }


    private void UpdateMesh()
    {
        if (_renderer == null)
        {
            _renderer = GetComponent<MeshRenderer>();
            _meshFilter = GetComponent<MeshFilter>();
        }

        // Prevent out of bounds warnings if no vertices to show
        if (Resolution < 1 || Repeat < 1)
        {
            _renderer.enabled = false;
            return;
        }

        _renderer.enabled = true;

        if (_mesh != null)
        {
            // destroy any old created mesh
            if (Application.isPlaying)
                Destroy(_mesh);
            else
                DestroyImmediate(_mesh);
            _mesh = null;
        }

        var doubleWithBothSided = BothSided ? 2 : 1;

        var vertexCount = (Resolution + 1) * Repeat * 2;

        var trianglePointCount = (Resolution) * Repeat * 2 * 3 * doubleWithBothSided;

        if (vertexCount < 1 || vertexCount > 10000)
        {
            Debug.LogWarning("Quadcount exceeds valid range of 1.. 10000:" + vertexCount);
            return;
        }

        _mesh = new Mesh();
        _mesh.name = "Circle";

        // resize arrays
        if (_cachedVertices == null || _cachedVertices.Length != vertexCount)
        {
            _cachedVertices = new Vector3[vertexCount];
        }

        if (_cachedUVs == null || _cachedUVs.Length != vertexCount)
        {
            _cachedUVs = new Vector2[vertexCount];
        }

        if (_cachedTrianglesPoints == null || _cachedTrianglesPoints.Length != trianglePointCount)
        {
            _cachedTrianglesPoints = new int[trianglePointCount];
        }

        var angle = StartAngle / 180 * Mathf.PI;
        var aStep = (FillAngle * FillRatio / 180 * Mathf.PI) / Resolution;
        var aGapStep = (FillAngle * (1 - FillRatio) / 180 * Mathf.PI);

        int vertexIndex = 0;
        int pointIndex = 0;
        Vector3 A1 = Vector3.zero, A2 = Vector3.zero, B1 = Vector3.zero, B2 = Vector3.zero;

        var offset = Mathf.Sin(Twist / 180 * Mathf.PI) * Width;
        var radialOffset = Mathf.Cos(Twist / 180 * Mathf.PI) * Width;
        var radiusA = Radius + radialOffset;
        var radiusB = Radius - radialOffset;

        float u = 0;
        float uStep = 1f / Resolution;

        for (var repeatIndex = 0; repeatIndex < Repeat; repeatIndex++)
        {
            var sinAngle1 = Mathf.Sin(angle);
            var cosAngle1 = Mathf.Cos(angle);
            float sinAngle2, cosAngle2;

            /*  NOTE: I guess you could optimize the code to avoid almost repeating the
                the following block. But I was optimizing for cycles and got lazy. */
            switch (_axis)
            {
                case Axis.X:
                    A1 = new Vector3(offset, sinAngle1 * radiusA, cosAngle1 * radiusA);
                    B1 = new Vector3(-offset, sinAngle1 * radiusB, cosAngle1 * radiusB);
                    break;

                case Axis.Y:
                    A1 = new Vector3(sinAngle1 * radiusA, offset, cosAngle1 * radiusA);
                    B1 = new Vector3(sinAngle1 * radiusB, -offset, cosAngle1 * radiusB);
                    break;

                case Axis.Z:
                    A1 = new Vector3(sinAngle1 * radiusA, cosAngle1 * radiusA, offset);
                    B1 = new Vector3(sinAngle1 * radiusB, cosAngle1 * radiusB, -offset);
                    break;
            }


            for (var qIndex = 0; qIndex < Resolution; qIndex++)
            {
                sinAngle2 = Mathf.Sin(angle + aStep);
                cosAngle2 = Mathf.Cos(angle + aStep);

                switch (_axis)
                {
                    case Axis.X:
                        A2 = new Vector3(offset, sinAngle2 * radiusA, cosAngle2 * radiusA);
                        B2 = new Vector3(-offset, sinAngle2 * radiusB, cosAngle2 * radiusB);
                        break;

                    case Axis.Y:
                        A2 = new Vector3(sinAngle2 * radiusA, offset, cosAngle2 * radiusA);
                        B2 = new Vector3(sinAngle2 * radiusB, -offset, cosAngle2 * radiusB);
                        break;

                    case Axis.Z:
                        A2 = new Vector3(sinAngle2 * radiusA, cosAngle2 * radiusA, offset);
                        B2 = new Vector3(sinAngle2 * radiusB, cosAngle2 * radiusB, -offset);
                        break;
                }
                /**
                   [+0]   [+2]
                   A1     A2
                    +---+ +  +---+  ..
                    |  / /|  |  /
                    | / / |  | /
                    +/ +--+  +/  ..
                   B1     B2
                   [+1]   [+3]
                 */

                _cachedVertices[vertexIndex] = A1;
                _cachedVertices[vertexIndex + 1] = B1;

                if (_textureMode == TextureMode.PerSegment)
                {
                    //u = angle / (Mathf.PI * 2);
                    u = (float) qIndex / Resolution;
                }


                if (_textureMode == TextureMode.Planar)
                {
                    _cachedUVs[vertexIndex] = new Vector2(sinAngle1 * radiusA, cosAngle1 * radiusA) / radiusA / 2 +
                                              Vector2.one * 0.5f;
                    _cachedUVs[vertexIndex + 1] = new Vector2(sinAngle1 * radiusB, cosAngle1 * radiusB) / radiusA / 2 +
                                                  Vector2.one * 0.5f;
                }
                else
                {
                    _cachedUVs[vertexIndex] = new Vector2(u, 1);
                    _cachedUVs[vertexIndex + 1] = new Vector2(u, 0);
                }

                _cachedTrianglesPoints[pointIndex + 0] = vertexIndex + 0;
                _cachedTrianglesPoints[pointIndex + 1] = vertexIndex + 2;
                _cachedTrianglesPoints[pointIndex + 2] = vertexIndex + 1;

                _cachedTrianglesPoints[pointIndex + 3] = vertexIndex + 2;
                _cachedTrianglesPoints[pointIndex + 4] = vertexIndex + 3;
                _cachedTrianglesPoints[pointIndex + 5] = vertexIndex + 1;

                if (BothSided)
                {
                    pointIndex += 6;
                    _cachedTrianglesPoints[pointIndex + 0] = vertexIndex + 1;
                    _cachedTrianglesPoints[pointIndex + 1] = vertexIndex + 2;
                    _cachedTrianglesPoints[pointIndex + 2] = vertexIndex + 0;

                    _cachedTrianglesPoints[pointIndex + 3] = vertexIndex + 1;
                    _cachedTrianglesPoints[pointIndex + 4] = vertexIndex + 3;
                    _cachedTrianglesPoints[pointIndex + 5] = vertexIndex + 2;
                }

                vertexIndex += 2;
                pointIndex += 6;

                u += uStep;

                // Shift value to reuse for next step
                sinAngle1 = sinAngle2;
                cosAngle1 = cosAngle2;
                A1 = A2;
                B1 = B2;

                // Compute next step
                angle += aStep;
            }

            // Close last triangles
            _cachedVertices[vertexIndex] = A1;
            _cachedVertices[vertexIndex + 1] = B1;

            if (_textureMode == TextureMode.PerSegment)
            {
                u = 1;
            }

            if (_textureMode == TextureMode.Planar)
            {
                _cachedUVs[vertexIndex] = new Vector2(sinAngle1 * radiusA, cosAngle1 * radiusA) / radiusA / 2 +
                                          Vector2.one * 0.5f;
                _cachedUVs[vertexIndex + 1] = new Vector2(sinAngle1 * radiusB, cosAngle1 * radiusB) / radiusA / 2 +
                                              Vector2.one * 0.5f;
            }
            else
            {
                _cachedUVs[vertexIndex] = new Vector2(u, 1);
                _cachedUVs[vertexIndex + 1] = new Vector2(u, 0);
            }

            vertexIndex += 2;

            angle += aGapStep;
        }

        _mesh.vertices = _cachedVertices;
        _mesh.uv = _cachedUVs;
        _mesh.triangles = _cachedTrianglesPoints;
        _meshFilter.sharedMesh = _mesh;
        _mesh.RecalculateBounds();
    }


    private MeshFilter _meshFilter;
    private Mesh _mesh;
    private MeshRenderer _renderer;

    private bool UpdateMeshIfParamsChanged()
    {
        if ((!_meshFilter.sharedMesh || _meshFilter.sharedMesh.vertexCount == 0)
            || _lastAxis != _axis
            || _lastRadius != Radius
            || _lastWidth != Width
            || _lastStartAngle != StartAngle
            || _lastFillAngle != FillAngle
            || _lastFillRatio != FillRatio
            || _lastRepeat != Repeat
            || _lastTwist != Twist
            || _lastResolution != Resolution
        )
        {
            InitializeParamsFromScriptProperties();
            UpdateMesh();
            return true;
        }

        return false;
    }

    private void InitializeParamsFromScriptProperties()
    {
        _lastAxis = _axis;
        _lastRadius = Radius;
        _lastWidth = Width;
        _lastStartAngle = StartAngle;
        _lastFillAngle = FillAngle;
        _lastFillRatio = FillRatio;
        _lastRepeat = Repeat;
        _lastTwist = Twist;
        _lastResolution = Resolution;
    }

    private Axis _lastAxis;
    private float _lastRadius;
    private float _lastWidth;
    private float _lastStartAngle;
    private float _lastFillAngle;
    private float _lastFillRatio;
    private int _lastRepeat;
    private float _lastTwist;
    private int _lastResolution;

    private Vector3[] _cachedVertices;
    private Vector2[] _cachedUVs;
    private int[] _cachedTrianglesPoints;


    public enum TextureMode
    {
        PerSegment,
        FullCircle,
        Planar,
    }

    public enum Axis
    {
        X,
        Y,
        Z,
    }
}