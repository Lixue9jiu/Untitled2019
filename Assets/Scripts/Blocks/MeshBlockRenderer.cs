using UnityEngine;
using System.Collections;

public class MeshBlockRenderer : IBlockRenderer
{
    private readonly MeshData m_mesh;

    private readonly Mesh standaloneMesh;

    public MeshBlockRenderer(Rect uv, Mesh mesh)
    {
        Vector2[] uvs = mesh.uv;
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = uvs[i] / 16f + uv.min;
        }
        mesh.uv = uvs;

        m_mesh = new MeshData(mesh, true);
        standaloneMesh = m_mesh.ToMesh();
    }

    public void DrawTerrain(BlockRenderContext context, Vector3Int pos, IMeshBuilder meshBuilder)
    {
        meshBuilder.Mesh(pos, m_mesh, Color.white);
    }

    public void DrawStandalone(int value, Matrix4x4 transform, Material sharedMaterial, BlockTextureManager textureManager, int layer, Camera camera)
    {
        sharedMaterial.mainTexture = textureManager.MainTexture;
        Graphics.DrawMesh(standaloneMesh, transform, sharedMaterial, layer, camera);
    }
}
