//  _____        _          ___                _                                                      
// |_   _| _  _ | |__  ___ | _ \ ___  _ _   __| | ___  _ _  ___  _ _                                  
//   | |  | || || '_ \/ -_)|   // -_)| ' \ / _` |/ -_)| '_|/ -_)| '_|                                 
//   |_|   \_,_||_.__/\___||_|_\\___||_||_|\__,_|\___||_|  \___||_|                                   
//                                                                                                    
//----------------------------------------------------------------------------------------------------

#region
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
#endregion

namespace MathBad
{
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TubeRenderer : MonoBehaviour
{
    [SerializeField] bool _stepSelf;
    [Min(3)]
    [SerializeField] int _sides = 6;
    [SerializeField] float _radius = 0.25f;
    [Range(0f, 360f)]
    [SerializeField] float _spin = 0f;
    [SerializeField] bool _taper = false;
    [SerializeField] bool _flipNormals = false;

    MeshFilter _meshFilter;
    MeshRenderer _meshRenderer;
    Mesh _mesh;

    Matrix4x4 _localToWorldMatrix;

    int _numPositions, _totalQuads;
    int _numVertices, _numIndices;
    float _totalLength;
    float _circumference;

    NativeArray<Vector3> _positions;
    NativeArray<Vector3> _vertices;
    NativeArray<Vector2> _uvs;
    NativeArray<int> _indices;

    bool _isDirty = false;
    bool _isDisposed = true;

    public bool isDisposed => _isDisposed;

    // Dispose
    //----------------------------------------------------------------------------------------------------
    void OnDisable() {Dispose();}
    public void Dispose()
    {
        if(_isDisposed)
            return;
        _isDisposed = true;
        _positions.Dispose();
        _vertices.Dispose();
        _uvs.Dispose();
        _indices.Dispose();
    }

    // Init
    //----------------------------------------------------------------------------------------------------
    public void Init(Vector3[] positions, float radius, int sides, bool flipNormals)
    {
        _radius = radius;
        _sides = sides;
        _flipNormals = flipNormals;

        _meshFilter = gameObject.GetComponent<MeshFilter>();
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();

        _mesh = _meshFilter.sharedMesh != null
                    ? _meshFilter.sharedMesh
                    : new Mesh();
        _mesh.name = "Tube";
        _meshFilter.mesh = _mesh;

        _numPositions = positions.Length;
        _numVertices = _numPositions * _sides;
        _numIndices = (_numPositions - 1) * _sides * 2 * 3;
        _totalQuads = (_numPositions - 1) * _sides;

        _positions = new NativeArray<Vector3>(positions, Allocator.Persistent);
        _vertices = new NativeArray<Vector3>(_numVertices, Allocator.Persistent);
        _uvs = new NativeArray<Vector2>(_numVertices, Allocator.Persistent);
        _indices = new NativeArray<int>(_numIndices, Allocator.Persistent);

        _circumference = (2f * Mathf.PI) * _radius;

        _isDisposed = false;
        _isDirty = true;
    }

    // Set & Get position
    //----------------------------------------------------------------------------------------------------
    public void SetPosition(int index, Vector3 position)
    {
        _positions[index] = position;
        _isDirty = true;
    }

    public Vector3 GetPosition(int index) => _positions[index];

    // MonoBehaviour
    //----------------------------------------------------------------------------------------------------
    void Update()
    {
        if(_stepSelf && _isDirty)
        {
            Step();
            _isDirty = false;
        }
    }

    // Step
    //----------------------------------------------------------------------------------------------------
    public Mesh Step()
    {
        _localToWorldMatrix = transform.localToWorldMatrix;
        JobHandle vertexJob = generateVerticesJob.Schedule(_numVertices, _sides);
        vertexJob.Complete();
        JobHandle uvsJob = generateUVsJob.Schedule(_numVertices, 128);
        uvsJob.Complete();

        JobHandle indicesJob = generateIndicesJob.Schedule(_totalQuads, 32);
        indicesJob.Complete();

        UpdateMesh();

        return _mesh;
    }

    void UpdateMesh()
    {
        _totalLength = 0f;
        for(int i = 0; i < _numPositions - 1; i++)
        {
            _totalLength += (_positions[i + 1] - _positions[i]).magnitude;
        }

        _mesh.SetVertices(_vertices);
        _mesh.SetTriangles(_indices.ToArray(), 0);
        _mesh.SetUVs(0, _uvs);

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
    }

    // Jobs
    //----------------------------------------------------------------------------------------------------
    GenerateVerticesJob generateVerticesJob
        => new GenerateVerticesJob
           {
               positions = _positions,
               vertices = _vertices,
               radius = _radius,
               sides = _sides,
               localToWorldMatrix = _localToWorldMatrix,
               taper = _taper,
           };

    GenerateUVsJob generateUVsJob
        => new GenerateUVsJob
           {
               uvs = _uvs,
               sides = _sides,
               circumference = _circumference,
               positions = _positions,
           };

    GenerateIndicesJob generateIndicesJob
        => new GenerateIndicesJob
           {
               indices = _indices,
               sides = _sides,
               flipNormals = _flipNormals,
           };

    [BurstCompile]
    struct GenerateVerticesJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        [ReadOnly] public NativeArray<Vector3> positions;
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<Vector3> vertices;

        [ReadOnly] public float radius;
        [ReadOnly] public int sides;
        [ReadOnly] public Matrix4x4 localToWorldMatrix;
        [ReadOnly] public bool taper;

        public void Execute(int index)
        {
            int segmentIndex = index / sides;
            int sideIndex = index % sides;
            int dirCount = 0;
            Vector3 forward = Vector3.zero;

            // not root
            if(segmentIndex > 0)
            {
                forward += (positions[segmentIndex] - positions[segmentIndex - 1]).normalized;
                dirCount++;
            }

            // not leaf
            if(segmentIndex < positions.Length - 1)
            {
                forward += (positions[segmentIndex + 1] - positions[segmentIndex]).normalized;
                dirCount++;
            }

            forward = dirCount > 0 ? (forward / dirCount).normalized : forward;

            Vector3 side = math.normalize(math.cross(forward, forward + new Vector3(.123564f, .34675f, .756892f)));
            Vector3 up = math.normalize(math.cross(forward, side));

            float angleStep = 2f * math.PI / sides;
            float angle = sideIndex * angleStep;
            float radiusFactor = 1f;
            if(taper) radiusFactor = 1f - (float)segmentIndex / (positions.Length - 1);
            Vector3 worldVertex = positions[segmentIndex] + (math.cos(angle) * radius * side + math.sin(angle) * radius * up) * radiusFactor;

            Matrix4x4 worldToLocalMatrix = localToWorldMatrix.inverse;
            Vector3 localVertex = worldToLocalMatrix.MultiplyPoint3x4(worldVertex);

            vertices[segmentIndex * sides + sideIndex] = localVertex;
        }
    }

    [BurstCompile]
    struct GenerateUVsJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<Vector2> uvs;
        [ReadOnly] public int sides;
        [ReadOnly] public float circumference;
        [ReadOnly] public NativeArray<Vector3> positions;

        public void Execute(int index)
        {
            float anglePerSide = (2f * Mathf.PI) / sides;
            float increment = 1f / sides;

            int segment = index / sides;
            int side = index % sides;

            float step = side * increment;
            float u = 0f;

            if(segment > 0)
            {
                for(int s = 1; s <= segment; s++)
                {
                    float segmentLength = math.distance(positions[s - 1], positions[s]);
                    u += segmentLength / circumference;
                }
            }

            float v = side * anglePerSide / (2f * math.PI);
            uvs[index] = new Vector2(u, v);
        }
    }

    [BurstCompile]
    struct GenerateIndicesJob : IJobParallelFor
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<int> indices;
        [ReadOnly] public int sides;
        [ReadOnly] public bool flipNormals;

        public void Execute(int quadIndex)
        {
            int segment = quadIndex / sides;
            int side = quadIndex % sides;
            int i = quadIndex * 6;

            int vertIndex = (segment + 1) * sides + side;
            int prevVertIndex = vertIndex - sides;
            if(flipNormals)
            {
                // Triangle one
                indices[i] = vertIndex;
                indices[i + 1] = (side + 1) % sides + (segment + 1) * sides;
                indices[i + 2] = prevVertIndex;

                // Triangle two
                indices[i + 3] = prevVertIndex;
                indices[i + 4] = (side + 1) % sides + (segment + 1) * sides;
                indices[i + 5] = prevVertIndex + 1 - (side == sides - 1 ? sides : 0);
            }
            else
            {
                // triangle one
                indices[i] = prevVertIndex;
                indices[i + 1] = (side + 1) % sides + (segment + 1) * sides;
                indices[i + 2] = vertIndex;

                // triangle two
                indices[i + 3] = prevVertIndex + 1 - (side == sides - 1 ? sides : 0);
                indices[i + 4] = (side + 1) % sides + (segment + 1) * sides;
                indices[i + 5] = prevVertIndex;
            }
        }
    }
}
}
