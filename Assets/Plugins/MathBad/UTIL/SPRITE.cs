#region Usings
using UnityEngine;
#endregion

namespace MathBad
{
public static class SPRITE
{
    public static Mesh GetSpriteMesh(this Sprite sprite)
    {
        // Vertices
        Vector2[] spriteVertices = sprite.vertices;
        Vector3[] meshVertices = new Vector3[spriteVertices.Length];
        for(int i = 0; i < spriteVertices.Length; i++)
        {
            meshVertices[i] = spriteVertices[i];
        }

        // Triangles
        ushort[] spriteTriangles = sprite.triangles;
        int[] meshTriangles = new int[spriteTriangles.Length];
        for(int i = 0; i < spriteTriangles.Length; i++)
            meshTriangles[i] = spriteTriangles[i];

        // Set UVs
        Vector2[] uvs = sprite.uv;

        // Apply
        Mesh mesh = new Mesh();
        mesh.name = $"{sprite.name} Mesh";
        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    public static MeshRenderer SpriteToMeshRenderer(SpriteRenderer sr, Material material)
    {
        Mesh mesh = sr.sprite.GetSpriteMesh();
        GameObject go = sr.gameObject;
        
        Object.Destroy(sr); // Have to remove SpriteRenderer

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = material;

        return mr;
    }
}
}
