using UnityEngine;

public class DefualtBlockRenderer : IBlockRenderer
{
    private bool[] m_useRandomVert;
    private Rect[] m_uvs;

    public DefualtBlockRenderer(Rect[] uvs, bool[] useRandomVert)
    {
        m_uvs = uvs;
        m_useRandomVert = useRandomVert;
    }

    public void Render(BlockRenderContext context, Vector3Int pos, IMeshBuilder meshBuilder)
    {
        int value = context.chunk[pos];
        int[] neighbors = GetNeighborsFast(context, pos);

        for (int i = 0; i < 6; i++)
        {
            if (value != 0 && value != neighbors[i] && !BlockRenderUtils.IsOpaque(context, neighbors[i]))
            {
                BlockRenderUtils.ShadedQuad(context, meshBuilder, pos, i, m_uvs[i], Color.white, Color.white, Color.white, Color.white, m_useRandomVert[i]);
            }
        }
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
