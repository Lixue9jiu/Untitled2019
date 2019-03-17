using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockRenderUtils
{
    private static readonly Color shadowIntensity = Color.white * 0.3f;

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
        bool[] n = GetNeighbors(context, context.origin + pos + offsets[face]);
        bool[] c = GetCorners(context, context.origin + pos);
        Vector3Int[] f = faces[face];
        Color[] colors = { ca, cb, cc, cd };
        int dim = face / 2;
        for (int i = 0; i < 4; i++)
        {
            int flag = 0;
            Color shadow = Color.white;
            for (int d = 0; d < 3; d++)
            {
                if (d != dim && n[d * 2 + 1 - f[i][d]])
                {
                    shadow -= shadowIntensity;
                    flag++;
                }
            }
            if (flag == 0 && c[f[i].x + (f[i].y << 1) + (f[i].z << 2)])
                shadow -= shadowIntensity;
            colors[i] *= shadowMasks[face];
            colors[i] *= shadow;
        }
        builder.Quad(pos + f[0], pos + f[1], pos + f[2], pos + f[3], tex, colors[0], colors[1], colors[2], colors[3], randomVert);
    }

    public static bool IsTransparent(BlockRenderContext context, int value)
    {
        return context.blockManager.IsTransparent(BlockData.GetContent(value));
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
            result[i] = !IsTransparent(context, values[i]);
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
            result[i] = !IsTransparent(context, values[i]);
        }
        return result;
    }
}
