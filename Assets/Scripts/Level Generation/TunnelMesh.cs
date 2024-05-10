#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Splines;
#endregion

[RequireComponent(typeof(TunnelGenerator))]
public class TunnelMesh : MonoBehaviour
{
    [SerializeField] bool _drawMesh;
    [SerializeField] float _vertexDensity = 0.1f;
    [SerializeField] float _verticalOffset = 2f;

    Mesh _tunnelMesh;
    GameObject _tube;
    bool _hasGenerated;
    Material _mat;
    public Mesh tunnelMesh => _tunnelMesh;

    public void GenerateMesh()
    {
        if(_hasGenerated || !Application.isPlaying)
            return;

        SplineContainer spline = GetComponent<SplineContainer>();

        float length = spline.CalculateLength();
        int numVerts = (length * _vertexDensity).FloorToInt();
        Vector3[] splinePositions = new Vector3[numVerts];

        for(int i = 0; i < numVerts; i++)
        {
            float t = mathi.Lerp(i, numVerts);
            Vector3 pos = spline.EvaluatePosition(t);
            splinePositions[i] = pos + new Vector3(0f, _verticalOffset, 0f);
        }

        Transform tube = transform.CreateChild("TunnelMesh");
        _tunnelMesh = MESH.Tube.Create(Matrix4x4.identity, splinePositions, 5f, 8, true);

        MeshCollider mc = tube.gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = _tunnelMesh;

        _mat = new Material(Shader.Find("MathBad/Wireframe"));
        _mat.SetColor("_WireColor", RGB.yellow);

        _hasGenerated = true;
    }

    public bool Raycast(Ray ray, out Vector3 hitPos, out Vector3 hitNormal)
    {
        // Raycast against the mesh using Ray -> Triangle intersection (Slow)
        // return _tunnelMesh.Raycast(ray, 2f, out hitPos, out hitNormal);

        bool success = Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f);
        hitPos = hit.point;
        hitNormal = hit.normal;
        return success;
    }

    void Update()
    {
        if(!_drawMesh || !_hasGenerated)
            return;

        Graphics.DrawMesh(_tunnelMesh, Matrix4x4.identity, _mat, 0, View.inst.mainCamera);
    }
}
