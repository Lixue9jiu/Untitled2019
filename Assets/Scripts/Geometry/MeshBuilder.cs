using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder : IMeshBuilder
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    public void Clear()
    {
        vertices.Clear();
        indices.Clear();
        uvs.Clear();
        colors.Clear();
    }

    public Mesh ToMesh()
    {
        return new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = indices.ToArray(),
            uv = uvs.ToArray(),
            colors = colors.ToArray()
        };
    }

    public void Mesh(Vector3Int pos, MeshData mesh, Color color)
    {
        int count = vertices.Count;
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            vertices.Add(mesh.vertices[i] + pos);
        }
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            indices.Add(mesh.triangles[i] + count);
        }

        uvs.AddRange(mesh.uv);
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            colors.Add(color);
        }
    }

    public void Quad(Vector3Int a, Vector3Int b, Vector3Int c, Vector3Int d, Rect tex, Color ca, Color cb, Color cc, Color cd, bool useRandomTex = false)
    {
        int count = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        vertices.Add(d);
        indices.Add(count);
        indices.Add(count + 1);
        indices.Add(count + 3);
        indices.Add(count + 1);
        indices.Add(count + 2);
        indices.Add(count + 3);

        if (useRandomTex)
        {
            var uvRect = new Vector2[] { tex.min, new Vector2(tex.xMin, tex.yMax), tex.max, new Vector2(tex.xMax, tex.yMin) };
            int r = NextRand(a.GetHashCode()) & 3;
            uvs.Add(uvRect[r]);
            uvs.Add(uvRect[(r + 1) & 3]);
            uvs.Add(uvRect[(r + 2) & 3]);
            uvs.Add(uvRect[(r + 3) & 3]);
        }
        else
        {
            uvs.Add(tex.min);
            uvs.Add(new Vector2(tex.xMin, tex.yMax));
            uvs.Add(tex.max);
            uvs.Add(new Vector2(tex.xMax, tex.yMin));
        }

        colors.Add(ca);
        colors.Add(cb);
        colors.Add(cc);
        colors.Add(cd);
    }

    private int NextRand(int randomSeed)
    {
        return (randomSeed * 1103515245 + 12345) & 0xFFFFFFF;
    }
}