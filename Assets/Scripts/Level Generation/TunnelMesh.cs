#region Usings
using UnityEngine;
using MathBad;
using UnityEngine.Splines;
#endregion

[RequireComponent(typeof(TunnelGenerator))]
public class TunnelMesh : MonoBehaviour
{
    [SerializeField] float _vertexDensity = 0.1f;

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
            splinePositions[i] = pos;
        }

        Transform tube = transform.CreateChild("TunnelMesh");
        TubeRenderer tr = tube.gameObject.AddComponent<TubeRenderer>();
        tr.Init(5f, 12, splinePositions);

        if(_tunnelMesh != null) DestroyImmediate(_tunnelMesh);
        _tunnelMesh = tr.Step();
        tr.Dispose();
        Destroy(tr);

        Material mat = new Material(Shader.Find("MathBad/Wireframe"));
        MeshRenderer mr = tube.GetComponent<MeshRenderer>();
        mr.material = mat;
        mr.material.SetColor("_WireColor", RGB.yellow);
    }

    public bool Raycast(Ray ray, out Vector3 hitPos, out Vector3 hitNormal)
        => _tunnelMesh.Raycast(ray, out hitPos, out hitNormal);
}
