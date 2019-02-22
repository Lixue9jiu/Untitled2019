using UnityEngine;
using Unity.Collections;
using System.Collections;

/// <summary>
/// A class that holds the data of a chunk
/// </summary>
public class Chunk
{
    public const int SIZE_X = 16;
    public const int SIZE_Y = 16;
    public const int SIZE_Z = 16;

    public const int SIZE_X_MINUS_ONE = SIZE_X - 1;
    public const int SIZE_Y_MINUS_ONE = SIZE_Y - 1;
    public const int SIZE_Z_MINUS_ONE = SIZE_Z - 1;

    public const int SHIFT_X = 4;
    public const int SHIFT_Y = 4;
    public const int SHIFT_Z = 4;

    private const int DATA_SHIFT_Y = SHIFT_X;
    private const int DATA_SHIFT_Z = SHIFT_Y + SHIFT_X;

    public static readonly Chunk airChunk = new Chunk();

    public int RenderIndex = -1;

    int[] cells;

    public Chunk()
    {
        cells = new int[SIZE_X * SIZE_Y * SIZE_Z];
    }

    public int this[int index]
    {
        get
        {
            return cells[index];
        }
        set
        {
            cells[index] = value;
        }
    }

    public int this[Vector3Int p]
    {
        get
        {
            return this[p.x, p.y, p.z];
        }
        set
        {
            this[p.x, p.y, p.z] = value;
        }
    }

    public int this[int x, int y, int z]
    {
        get
        {
            return cells[GetIndex(x, y, z)];
        }
        set
        {
            cells[GetIndex(x, y, z)] = value;
        }
    }

    public static int GetIndex(int x, int y, int z)
    {
        return x + (y << DATA_SHIFT_Y) + (z << DATA_SHIFT_Z);
    }

    public NativeArray<int> ToNativeArray(Allocator alloc)
    {
        return new NativeArray<int>(cells, alloc);
    }
}
