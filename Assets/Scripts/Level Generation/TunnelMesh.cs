#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Splines;
#endregion

[RequireComponent(typeof(TunnelGenerator))]
public class TunnelMesh : MonoBehaviour
{
    [SerializeField] bool _renderMesh;
    [SerializeField] float _vertexDensity = 0.1f;
    [SerializeField] float _verticalOffset = 2f;

    Mesh _tunnelMesh;
    GameObject _tube;
    public Mesh tunnelMesh => _tunnelMesh;

    public void GenerateMesh()
    {
        if(!Application.isPlaying)
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
        TubeRenderer tr = tube.gameObject.AddComponent<TubeRenderer>();
        tr.Init(splinePositions, 5f, 8, true);

        if(_tunnelMesh != null)
            DestroyImmediate(_tunnelMesh);

        _tunnelMesh = tr.Step();
        tr.Dispose();

        MeshCollider mc = tube.gameObject.AddComponent<MeshCollider>();
        mc.sharedMesh = _tunnelMesh;

        Material mat = new Material(Shader.Find("MathBad/Wireframe"));
        MeshRenderer mr = tube.GetComponent<MeshRenderer>();
        mr.material = mat;
        mr.material.SetColor("_WireColor", RGB.yellow);
        if(!_renderMesh) mr.enabled = false;
    }

    public bool Raycast(Ray ray, out Vector3 hitPos, out Vector3 hitNormal)
    {
        // return _tunnelMesh.Raycast(ray, 2f, out hitPos, out hitNormal);

        bool success = Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 100f);
        hitPos = hit.point;
        hitNormal = hit.normal;
        return success;
    }
}
