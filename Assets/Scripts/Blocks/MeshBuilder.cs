using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder : IMeshBuilder
{
    /// <summary>
    /// The vertices defining six faces in the order of: x+, x-, y+, y-, z+, z-
    /// </summary>
    private static Vector3Int[][] faces =
    {
        new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0), new Vector3Int(1, 1, 1), new Vector3Int(1, 0, 1) },
        new Vector3Int[] { new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 1), new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 0) },
        new Vector3Int[] { new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 1) },
        new Vector3Int[] { new Vector3Int(1, 0, 1), new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0) },
        new Vector3Int[] { new Vector3Int(1, 0, 1), new Vector3Int(1, 1, 1), new Vector3Int(0, 1, 1), new Vector3Int(0, 0, 1) },
        new Vector3Int[] { new Vector3Int(0, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(1, 0, 0) }
    };

    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

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

    public void Quad(Vector3Int pos, int face, Rect tex, Color ca, Color cb, Color cc, Color cd)
    {
        var f = faces[face];
        int count = vertices.Count;
        vertices.Add(f[0] + pos);
        vertices.Add(f[1] + pos);
        vertices.Add(f[2] + pos);
        vertices.Add(f[3] + pos);
        indices.Add(count);
        indices.Add(count + 1);
        indices.Add(count + 3);
        indices.Add(count + 1);
        indices.Add(count + 2);
        indices.Add(count + 3);

        uvs.Add(tex.min);
        uvs.Add(new Vector2(tex.xMin, tex.yMax));
        uvs.Add(tex.max);
        uvs.Add(new Vector2(tex.xMax, tex.yMin));

        colors.Add(ca);
        colors.Add(cb);
        colors.Add(cc);
        colors.Add(cd);
    }
}