using UnityEngine;
using System.Collections;

public class DefualtBlockRenderer : IBlockRenderer
{
    private static readonly Color shadowIntensity = Color.black * 0.3f;
    private static readonly Vector3Int[] faces = {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1)
    };
    private Rect[] m_uvs;

    public DefualtBlockRenderer(Rect[] uvs)
    {
        m_uvs = uvs;
    }

    public void Render(BlockRenderContext context, Vector3Int pos, IMeshBuilder meshBuilder)
    {
        int value = context.chunk[pos];
        int[] neighbors = GetNeighborsFast(context, pos);

        for (int i = 0; i < 6; i++)
        {
            if (value != 0 && value != neighbors[i] && !IsOpaque(context, neighbors[i]))
            {
                bool[] n = GetNeighbors(context, context.origin + pos + faces[i]);
                bool[] c = GetCorners(context, context.origin + pos);
                meshBuilder.ShadedQuad(pos, i, m_uvs[i], Color.white, Color.white, Color.white, Color.white, n, c);
            }
        }
    }

    private static bool IsOpaque(BlockRenderContext context, int value)
    {
        return context.blockManager.IsOpaque(BlockData.GetContent(value));
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

    private static bool[] GetNeighbors(BlockRenderContext context, Vector3Int pos)
    {
        var values = new int[] {
            context.terrainManager.GetCellValue(pos.x + 1, pos.y, pos.z),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y + 1, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y - 1, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x, pos.y, pos.z - 1),
        };
        bool[] result = new bool[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = IsOpaque(context, values[i]);
        }
        return result;
    }

    private static bool[] GetCorners(BlockRenderContext context, Vector3Int pos)
    {
        var values = new int[] {
            context.terrainManager.GetCellValue(pos.x - 1, pos.y - 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y - 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y + 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y + 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y - 1, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y - 1, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y + 1, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y + 1, pos.z + 1),
        };
        bool[] result = new bool[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = IsOpaque(context, values[i]);
        }
        return result;
    }
}
