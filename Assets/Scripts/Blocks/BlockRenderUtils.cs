using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockRenderUtils
{
    private const float m_ShadowMask = 0.3f;
    private const float m_AmbiantLight = 0.5f;

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

    public static Vector3 mainLight = new Vector3(-1.2f, -2, -1).normalized;

    public static void Quad(IMeshBuilder builder, int face, Rect tex, Color ca, Color cb, Color cc, Color cd)
    {
        Vector3Int[] f = faces[face];
        builder.Quad(f[0], f[1], f[2], f[3], tex, ca, cb, cc, cd, MeshBuilderFlags.None);
    }

    public static void ShadedQuad(BlockRenderContext context, IMeshBuilder builder, Vector3Int pos, int face, Rect tex, Color ca, Color cb, Color cc, Color cd, bool randomVert)
    {
        NeighborInfo[] n = GetNeighbors(context, context.origin + pos + offsets[face]);
        NeighborInfo[] c = GetCorners(context, context.origin + pos);
        Vector3Int[] f = faces[face];
        Color[] colors = { ca, cb, cc, cd };
        int shadowCondition = 0;
        int dim = face / 2;

        // for each vertex
        for (int i = 0; i < 4; i++)
        {
            bool isCorner = true;
            float sideShadow = 0;
            float finalShadow = 0;
            for (int d = 0; d < 3; d++)
            {
                // check neighbors of the transparent block above the face
                if (d != dim)
                {
                    var current = n[d * 2 + 1 - f[i][d]];
                    sideShadow += current.shadowLength;
                    isCorner &= current.isTransparent;
                }
            }
            // check corners of the original block (could be optimized to check only the corners of the face)
            var corner = c[f[i].x + (f[i].y << 1) + (f[i].z << 2)];

            // multiple each vertex with the shadow length of the block above the face
            float topShadow = GetShadowLength(context, context.terrainManager.GetCellValue(context.origin + pos + offsets[face]));

            if (sideShadow < corner.shadowLength)
            {
                shadowCondition += 1 << i;
            }

            if (isCorner || corner.isTransparent)
            {
                finalShadow = corner.shadowLength + sideShadow + topShadow;
            }
            else
            {
                finalShadow = sideShadow + topShadow;
            }

            colors[i] *= CalculateShadow(mainLight, offsets[face]);
            colors[i] *= Color.white * (1 - finalShadow * m_ShadowMask);
        }
        var flags = MeshBuilderFlags.None;
        if (randomVert)
            flags |= MeshBuilderFlags.UseRandomVertices;
        if (shadowCondition == 2 || shadowCondition == 8 || shadowCondition == 11)
            flags |= MeshBuilderFlags.InvertTriangles;
        builder.Quad(pos + f[0], pos + f[1], pos + f[2], pos + f[3], tex, colors[0], colors[1], colors[2], colors[3], flags);
    }

    public static Color CalculateShadow(Vector3 light, Vector3 normal)
    {
        return Color.white * Mathf.Lerp(m_AmbiantLight, 1f, (-Vector3.Dot(light, normal) + 1) / 2);
    }

    public static bool IsTransparent(BlockRenderContext context, int value)
    {
        return context.blockManager.IsTransparent(BlockData.GetContent(value));
    }

    public static float GetShadowLength(BlockRenderContext context, int value)
    {
        return context.blockManager.Blocks[BlockData.GetContent(value)].shadowStrength;
    }

    struct NeighborInfo
    {
        public float shadowLength;
        public bool isTransparent;
    }

    private static NeighborInfo[] GetNeighbors(BlockRenderContext context, Vector3Int pos)
    {
        var values = new int[] {
            context.terrainManager.GetCellValue(pos.x + 1, pos.y, pos.z),
            context.terrainManager.GetCellValue(pos.x - 1, pos.y, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y + 1, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y - 1, pos.z),
            context.terrainManager.GetCellValue(pos.x, pos.y, pos.z + 1),
            context.terrainManager.GetCellValue(pos.x, pos.y, pos.z - 1),
        };
        NeighborInfo[] result = new NeighborInfo[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i].shadowLength = GetShadowLength(context, values[i]);
            result[i].isTransparent = IsTransparent(context, values[i]);
        }
        return result;
    }

    // get the transparency of the blocks at the 8 corners of a block
    private static NeighborInfo[] GetCorners(BlockRenderContext context, Vector3Int pos)
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
        NeighborInfo[] result = new NeighborInfo[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i].shadowLength = GetShadowLength(context, values[i]);
            result[i].isTransparent = IsTransparent(context, values[i]);
        }
        return result;
    }
}
