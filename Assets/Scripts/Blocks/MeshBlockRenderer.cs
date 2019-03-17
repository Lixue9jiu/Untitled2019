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
        //int value = context.chunk[pos];
        //int[] neighbors = GetNeighborsFast(context, pos);

        //for (int i = 0; i < 6; i++)
        //{
        //    if (value != neighbors[i] && BlockRenderUtils.IsTransparent(context, neighbors[i]))
        //    {
        //        BlockRenderUtils.ShadedQuad(context, meshBuilder, pos, i, m_uv, Color.white, Color.white, Color.white, Color.white, false);
        //    }
        //}
        meshBuilder.Mesh(pos, m_mesh, Color.white);
    }

    private static int[] GetNeighborsFast(BlockRenderContext context, Vector3Int pos)
    {
        return new int[] {
            pos.x == Chunk.SIZE_X_MINUS_ONE ? context.neighbors[0][0, pos.y, pos.z] : context.chunk[pos.x + 1, pos.y, pos.z],
            pos.x == 0 ? context.neighbors[1][Chunk.SIZE_X_MINUS_ONE, pos.y, pos.z] : context.chunk[pos.x - 1, pos.y, pos.z],
            pos.y == Chunk.SIZE_Y_MINUS_ONE ? context.neighbors[2][pos.x, 0, pos.z] : context.chunk[pos.x, pos.y + 1, pos.z],
            pos.y == 0 ? context.neighbors[3][pos.x, Chunk.SIZE_Y_MINUS_ONE, pos.z] : context.chunk[pos.x, pos.y - 1, pos.z],
            pos.z == Chunk.SIZE_Z_MINUS_ONE ? context.neighbors[4][pos.x, pos.y, 0] : context.chunk[pos.x, pos.y, pos.z + 1],
            pos.z == 0 ? context.neighbors[5][pos.x, pos.y, Chunk.SIZE_Z_MINUS_ONE] : context.chunk[pos.x, pos.y, pos.z - 1]
        };
    }
}
