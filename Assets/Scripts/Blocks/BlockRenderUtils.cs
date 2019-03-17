using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockRenderUtils
{
    private static readonly Color m_ShadowMask = Color.white * 0.3f;

    /// <summary>
    /// The vertices defining six faces in the order of: x+, x-, y+, y-, z+, z-
    /// </summary>
    private static readonly Vector3Int[][] faces =
    {
        new Vector3Int[] { new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0), new Vector3Int(1, 1, 1), new Vector3Int(1, 0, 1) },
        new Vector3Int[] { new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 1), new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 0) },
        new Vector3Int[] { new Vector3Int(1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 1) },
        new Vector3Int[] { new Vector3Int(1, 0, 1), new Vector3Int(0, 0, 1), new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0) },
        new Vector3Int[] { new Vector3Int(1, 0, 1), new Vector3Int(1, 1, 1), new Vector3Int(0, 1, 1), new Vector3Int(0, 0, 1) },
        new Vector3Int[] { new Vector3Int(0, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(1, 0, 0) }
    };

    private static readonly Vector3Int[] offsets =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1)
    };

    private static readonly Color[] shadowMasks =
    {
        Color.white * 0.9f,
        Color.white * 0.8f,
        Color.white,
        Color.white * 0.6f,
        Color.white * 0.8f,
        Color.white * 0.7f
    };

    public static void ShadedQuad(BlockRenderContext context, IMeshBuilder builder, Vector3Int pos, int face, Rect tex, Color ca, Color cb, Color cc, Color cd, bool randomVert)
    {
        float[] n = GetNeighbors(context, context.origin + pos + offsets[face]);
        float[] c = GetCorners(context, context.origin + pos);
        Vector3Int[] f = faces[face];
        Color[] colors = { ca, cb, cc, cd };
        int shadowCondition = 0;
        int dim = face / 2;

        // for each vertex
        for (int i = 0; i < 4; i++)
        {
            int flag = 0;
            Color shadow = Color.white;
            for (int d = 0; d < 3; d++)
            {
                // check neighbors of the transparent block above the face
                if (d != dim)
                {
                    float s = n[d * 2 + 1 - f[i][d]];
                    if (s > 0)
                    {
                        shadow -= m_ShadowMask * s;
                        flag++;
                    }
                }
            }
            // check corners of the original block (could be optimized to check only the corners of the face)
            if (flag == 0)
            {
                float s = c[f[i].x + (f[i].y << 1) + (f[i].z << 2)];
                if (s > 0)
                {
                    shadow -= m_ShadowMask * s;
                    shadowCondition += 1 << i;
                }
            }
            // multiple each vertex with the shadow length of the block above the face
            colors[i] *= shadowMasks[face] - m_ShadowMask * GetShadowLength(context, context.terrainManager.GetCellValue(context.origin + pos + offsets[face]));
            colors[i] *= shadow;
        }
        var flags = MeshBuilderFlags.None;
        if (randomVert)
            flags |= MeshBuilderFlags.UseRandomVertices;
        if (shadowCondition == 2 || shadowCondition == 8 || shadowCondition == 11)
            flags |= MeshBuilderFlags.InvertTriangles;
        builder.Quad(pos + f[0], pos + f[1], pos + f[2], pos + f[3], tex, colors[0], colors[1], colors[2], colors[3], flags);
    }

    public static bool IsTransparent(BlockRenderContext context, int value)
    {
        return context.blockManager.IsTransparent(BlockData.GetContent(value));
    }

    public static float GetShadowLength(BlockRenderContext context, int value)
    {
        return context.blockManager.Blocks[BlockData.GetContent(value)].shadowStrength;
    }

    private static float[] GetNeighbors(BlockRenderContext context, Vector3Int pos)
    {
        var values = new int[] {
            context.terrainManager.GetCellValue(pos.x + 1, pos.y, pos.z),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y + 1, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y - 1, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x, pos.y, pos.z - 1),
        };
        float[] result = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = GetShadowLength(context, values[i]);
        }
        return result;
    }

    // get the transparency of the blocks at the 8 corners of a block
    private static float[] GetCorners(BlockRenderContext context, Vector3Int pos)
    {
        var values = new int[] {
            context.terrainManager.GetCellValue(pos.x - 1, pos.y - 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y - 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y + 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y + 1, pos.z - 1),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y - 1, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y - 1, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y + 1, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x + 1, pos.y + 1, pos.z + 1)
        };
        float[] result = new float[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = GetShadowLength(context, values[i]);
        }
        return result;
    }
}
