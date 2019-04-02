using UnityEngine;
using System.Collections.Generic;

public class CharacterBody2 : MonoBehaviour
{
    BlockManager blockManager;
    TerrainRenderer terrainRenderer;
    TerrainManager terrainManager;

    Vector3 moveOrder;
    [SerializeField]
    Bounds bounds;
    [SerializeField]
    float skinThickness = 1f;

    private void Awake()
    {
        blockManager = FindObjectOfType<BlockManager>();
        terrainRenderer = FindObjectOfType<TerrainRenderer>();
        terrainManager = FindObjectOfType<TerrainManager>();
    }

    private void Start()
    {
        bounds.center = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    public HitResult Move(Vector3 order)
    {
        HitResult flag = Move2(ref order);
        bounds.center += order;
        transform.position += order;
        return flag;
    }

    void CollisionTestX(ref float value, ref HitResult result)
    {
        Vector3Int min = Vector3Int.FloorToInt(bounds.min);
        Vector3Int max = Vector3Int.CeilToInt(bounds.max);

        int maxx = Mathf.CeilToInt(bounds.max.x);
        int minx = Mathf.FloorToInt(bounds.min.x) - 1;

        float finalOffset = 0;
        for (int y = min.y; y < max.y; y++)
        {
            for (int z = min.z; z < max.z; z++)
            {
                var b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(minx, y, z))];
                if (b.HasValue)
                {
                    var offset = b.Value.bounds.max.x - (bounds.min.x - minx + value);
                    if (offset + skinThickness > 0)
                    {
                        result |= HitResult.Back;
                        if (value < 0)
                            finalOffset = Mathf.Max(finalOffset, offset);
                    }
                }
                b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(maxx, y, z))];
                if (b.HasValue)
                {
                    var offset = b.Value.bounds.min.x - (bounds.max.x - maxx + value);
                    if (offset - skinThickness < 0)
                    {
                        result |= HitResult.Front;
                        if (value > 0)
                            finalOffset = Mathf.Min(finalOffset, offset);
                    }
                }
            }
        }
        value += finalOffset;
    }

    void CollisionTestY(ref float value, ref HitResult result)
    {
        Vector3Int min = Vector3Int.FloorToInt(bounds.min);
        Vector3Int max = Vector3Int.CeilToInt(bounds.max);

        int maxy = Mathf.CeilToInt(bounds.max.y);
        int miny = Mathf.FloorToInt(bounds.min.y) - 1;

        float finalOffset = 0;
        for (int x = min.x; x < max.x; x++)
        {
            for (int z = min.z; z < max.z; z++)
            {
                var b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(x, miny, z))];
                if (b.HasValue)
                {
                    var offset = b.Value.bounds.max.y - (bounds.min.y - miny + value);
                    if (offset + skinThickness > 0)
                    {
                        result |= HitResult.Floor;
                        if (value < 0)
                            finalOffset = Mathf.Max(finalOffset, offset);
                    }
                }
                b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(x, maxy, z))];
                if (b.HasValue)
                {
                    var offset = b.Value.bounds.min.y - (bounds.max.y - maxy + value);
                    if (offset - skinThickness < 0)
                    {
                        result |= HitResult.Ceiling;
                        if (value > 0)
                            finalOffset = Mathf.Min(finalOffset, offset);
                    }
                }
            }
        }
        value += finalOffset;
    }

    void CollisionTestZ(ref float value, ref HitResult result)
    {
        Vector3Int min = Vector3Int.FloorToInt(bounds.min);
        Vector3Int max = Vector3Int.CeilToInt(bounds.max);

        int maxz = Mathf.FloorToInt(bounds.max.z);
        int minz = Mathf.FloorToInt(bounds.min.z);

        Vector3 off = new Vector3(0, 0, value);
        bounds.center += off;
        float finalOffset = 0;
        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                bounds.Expand(skinThickness);
                Vector3 basis = new Vector3(x, y, minz);
                bounds.center -= basis;
                var b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(x, y, minz))];
                if (b.HasValue)
                {
                    if (bounds.Intersects(b.Value.bounds))
                    {
                        result |= HitResult.Right;
                        finalOffset = Mathf.Max(finalOffset, b.Value.bounds.max.z - bounds.min.z - skinThickness);
                    }
                }
                bounds.center += basis;

                basis = new Vector3(x, y, maxz);
                bounds.center -= basis;
                b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(x, y, maxz))];
                if (b.HasValue)
                {
                    if (bounds.Intersects(b.Value.bounds))
                    {
                        result |= HitResult.Left;
                        finalOffset = Mathf.Min(finalOffset, b.Value.bounds.min.z - bounds.max.z + skinThickness);
                    }
                }
                bounds.center += basis;
                bounds.Expand(-skinThickness);
            }
        }
        bounds.center -= off;
        value += finalOffset;
    }

    void CollisionTest(ref Vector3 order, out HitResult result)
    {
        Vector3Int min = Vector3Int.FloorToInt(bounds.min);
        Vector3Int max = Vector3Int.CeilToInt(bounds.max);

        result = 0;
        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                for (int z = min.z; z < max.z; z++)
                {
                    Vector3 basis = new Vector3(x, y, z);
                    bounds.center -= basis;
                    var b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(x, y, z))];
                    if (b.HasValue)
                    {
                        var bounds2 = b.Value.bounds;
                        //bounds.Expand(new Vector3(skinThickness, 0, 0));
                        //bounds.center += new Vector3(order.x, 0, 0);
                        //if (bounds.Intersects(bounds2))
                        //{
                        //    result |= order.x > 0 ? HitResult.Front : HitResult.Back;
                        //    order.x = 0;
                        //}
                        //bounds.center -= new Vector3(order.x, 0, 0);
                        //bounds.Expand(new Vector3(-skinThickness, 0, 0));

                        //bounds.extents += new Vector3(0, skinThickness, 0);
                        var v = new Vector3(0, order.y, 0);
                        bounds.center += v;
                        if (bounds.Intersects(bounds2))
                        {
                            result |= order.y < 0 ? HitResult.Ceiling : HitResult.Floor;
                            order.y = 0;
                        }
                        bounds.center -= v;
                        //bounds.extents -= new Vector3(0, skinThickness, 0);

                        //bounds.Expand(new Vector3(0, 0, skinThickness));
                        //bounds.center += new Vector3(0, 0, order.z);
                        //if (bounds.Intersects(bounds2))
                        //{
                        //    result |= order.x > 0 ? HitResult.Left : HitResult.Right;
                        //    order.z = 0;
                        //}
                        //bounds.center -= new Vector3(0, 0, order.z);
                        //bounds.Expand(new Vector3(0, 0, -skinThickness));
                    }
                    bounds.center += basis;
                }
            }
        }
    }

    private void CollisionTest2(Vector3 order, out Vector3 fix, out HitResult result)
    {
        Vector3Int min = Vector3Int.FloorToInt(bounds.min);
        Vector3Int max = Vector3Int.CeilToInt(bounds.max);

        fix = Vector3.zero;
        result = 0;
        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                for (int z = min.z; z < max.z; z++)
                {
                    var b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(x, y, z))];
                    if (b.HasValue)
                    {
                        Vector3 f;
                        var block = new Bounds(b.Value.bounds.center + new Vector3(x, y, z), b.Value.bounds.size);
                        result |= CheckCollision(bounds, block, order, out f);
                        if (f.sqrMagnitude > fix.sqrMagnitude)
                            fix = f;
                    }
                }
            }
        }
    }

    private HitResult CheckCollision(Bounds k, Bounds r, Vector3 order, out Vector3 fix)
    {
        if (k.Intersects(r))
        {
            Vector3 a = -order.normalized;

            int index = -1;
            float t = float.PositiveInfinity;
            for (int i = 0; i < 3; i++)
            {
                var v = (a[i] > 0 ? (r.max[i] - k.min[i]) : (r.min[i] - k.max[i])) / a[i];
                if (v < t)
                {
                    t = v;
                    index = i;
                }
            }

            fix = a * t;
            return (HitResult)(1 << (index * 2 + (a[index] < 0 ? 0 : 1)));
        }
        fix = Vector3.zero;
        return 0;
    }

    private HitResult Move2(ref Vector3 order)
    {
        Vector3Int min = Vector3Int.FloorToInt(bounds.min);
        Vector3Int max = Vector3Int.CeilToInt(bounds.max);

        Vector3 allFix = Vector3.zero;
        HitResult result = 0;
        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                for (int z = min.z; z < max.z; z++)
                {
                    var b = terrainRenderer.BlockBounds[BlockData.GetContent(terrainManager.GetCellValue(x, y, z))];
                    if (b.HasValue)
                    {
                        var block = new Bounds(b.Value.bounds.center + new Vector3(x, y, z), b.Value.bounds.size);
                        float fix;
                        bounds.center += new Vector3(order.x, 0, 0);
                        result |= MoveX(bounds, block, order.x, out fix);
                        if (fix > allFix.x)
                            allFix.x = fix;
                        bounds.center -= new Vector3(order.x, 0, 0);

                        bounds.center += new Vector3(0, order.y, 0);
                        result |= MoveY(bounds, block, order.y, out fix);
                        if (fix > allFix.y)
                            allFix.y = fix;
                        bounds.center -= new Vector3(0, order.y, 0);

                        bounds.center += new Vector3(0, 0, order.z);
                        result |= MoveZ(bounds, block, order.z, out fix);
                        if (fix > allFix.z)
                            allFix.z = fix;
                        bounds.center -= new Vector3(0, 0, order.z);
                    }
                }
            }
        }
        order += allFix;
        return result;
    }

    private HitResult MoveY(Bounds k, Bounds r, float value, out float fix)
    {
        if (k.Intersects(r))
        {
            fix = value < 0 ? (r.max.y - k.min.y) : (r.min.y - k.max.y);
            return value > 0 ? HitResult.Ceiling : HitResult.Floor;
        }
        fix = 0;
        return 0;
    }

    private HitResult MoveX(Bounds k, Bounds r, float value, out float fix)
    {
        if (k.Intersects(r))
        {
            fix = value < 0 ? (r.max.x - k.min.x) : (r.min.x - k.max.x);
            return value > 0 ? HitResult.Front : HitResult.Back;
        }
        fix = 0;
        return 0;
    }

    private HitResult MoveZ(Bounds k, Bounds r, float value, out float fix)
    {
        if (k.Intersects(r))
        {
            fix = value < 0 ? (r.max.z - k.min.z) : (r.min.z - k.max.z);
            return value > 0 ? HitResult.Left : HitResult.Right;
        }
        fix = 0;
        return 0;
    }

    [System.Flags]
    public enum HitResult
    {
        Front = 1,
        Back = 2,
        Ceiling = 4,
        Floor = 8,
        Left = 16,
        Right = 32
    }
}
