//  __  __  ___  ___  _  _                                                                            
// |  \/  || __|/ __|| || |                                                                           
// | |\/| || _| \__ \| __ |                                                                           
// |_|  |_||___||___/|_||_|                                                                           
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
public static class MESH
{
    static MESH()
    {
        quadXZ = Resources.Load<Mesh>("MathBad/Mesh/QuadXZ");
        quadXY_32X32 = Resources.Load<Mesh>("MathBad/Mesh/QuadXY_32x32");
        quadXY_64X64 = Resources.Load<Mesh>("MathBad/Mesh/QuadXY_64x64");
    }

    public static readonly Mesh quadXZ;
    public static readonly Mesh quadXY_32X32, quadXY_64X64;

#region Tube
    public static class Tube
    {
        public static Mesh Create(Matrix4x4 localToWorldMatrix, Vector3[] line, float radius, int sides, bool faceInside, bool taper = false)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Tube";

            int numPositions = line.Length;
            int numVertices = numPositions * sides;
            int numIndices = (numPositions - 1) * sides * 2 * 3;
            int totalQuads = (numPositions - 1) * sides;

            NativeArray<Vector3> positions = new NativeArray<Vector3>(line, Allocator.Persistent);
            NativeArray<Vector3> vertices = new NativeArray<Vector3>(numVertices, Allocator.Persistent);
            NativeArray<Vector2> uvs = new NativeArray<Vector2>(numVertices, Allocator.Persistent);
            NativeArray<int> indices = new NativeArray<int>(numIndices, Allocator.Persistent);

            float circumference = 2f * Mathf.PI * radius;

            // Run jobs
            JobHandle vertexJob = new GenerateVerticesJob
                                  {
                                      positions = positions,
                                      vertices = vertices,
                                      radius = radius,
                                      sides = sides,
                                      localToWorldMatrix = localToWorldMatrix,
                                      taper = taper,
                                  }.Schedule(numVertices, sides);
            vertexJob.Complete();
            JobHandle uvsJob = new GenerateUVsJob
                               {
                                   uvs = uvs,
                                   sides = sides,
                                   circumference = circumference,
                                   positions = positions,
                               }.Schedule(numVertices, 128);
            uvsJob.Complete();
            JobHandle indicesJob = new GenerateIndicesJob
                                   {
                                       indices = indices,
                                       sides = sides,
                                       flipNormals = faceInside,
                                   }.Schedule(totalQuads, 32);
            indicesJob.Complete();

            mesh.SetVertices(vertices);
            mesh.SetTriangles(indices.ToArray(), 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Dipose
            positions.Dispose();
            vertices.Dispose();
            uvs.Dispose();
            indices.Dispose();

            return mesh;
        }

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
#endregion
}
}
