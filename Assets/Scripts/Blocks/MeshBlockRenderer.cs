using UnityEngine;
using System.Collections;

public class MeshBlockRenderer : IBlockRenderer
{
    private readonly MeshData m_mesh;

    public MeshBlockRenderer(Rect uv, Mesh mesh)
    {
        Vector2[] uvs = mesh.uv;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = uvs[i] / 16f + uv.min;
        }
        mesh.uv = uvs;

        m_mesh = new MeshData(mesh);
    }

    public void Render(BlockRenderContext context, Vector3Int pos, IMeshBuilder meshBuilder)
    {
        meshBuilder.Mesh(pos, m_mesh, Color.white);
    }
}
