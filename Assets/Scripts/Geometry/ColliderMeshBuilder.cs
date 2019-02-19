using UnityEngine;
using System.Collections.Generic;

public class ColliderMeshBuilder
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> indices = new List<int>();

    public Mesh ToMesh()
    {
        return new Mesh
        {
            vertices = vertices.ToArray(),
            triangles = indices.ToArray()
        };
    }

    public void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
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
    }

    public void Box(Vector3 pos, Bounds bounds)
    {
        
    }
}
