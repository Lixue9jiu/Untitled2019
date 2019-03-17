using UnityEngine;
using System;

public interface IMeshBuilder
{
    /// <summary>
    /// Add an rectangle to the mesh, with the given position, face, uv, and color
    /// </summary>
    /// <param name="pos">Position.</param>
    /// <param name="face">Face.</param>
    /// <param name="tex">Tex.</param>
    /// <param name="ca">Ca.</param>
    /// <param name="cb">Cb.</param>
    /// <param name="cc">Cc.</param>
    /// <param name="cd">Cd.</param>
    void Quad(Vector3Int a, Vector3Int b, Vector3Int c, Vector3Int d, Rect tex, Color ca, Color cb, Color cc, Color cd, MeshBuilderFlags flags);
    void Mesh(Vector3Int pos, MeshData mesh, Color color);
}

[Flags]
public enum MeshBuilderFlags : short
{
    None = 0,
    UseRandomVertices = 1,
    InvertTriangles = 2
}