using UnityEngine;
using System.Collections;

/// <summary>
/// A class that holds the data of a chunk
/// </summary>
public class Chunk
{
    public const int SIZE_X = 16;
    public const int SIZE_Y = 128;
    public const int SIZE_Z = 16;

    public const int SIZE_X_MINUS_ONE = 15;
    public const int SIZE_Y_MINUS_ONE = 127;
    public const int SIZE_Z_MINUS_ONE = 15;

    const int SHIFT_Y = 4;
    const int SHIFT_Z = SHIFT_Y + 7;

    public static readonly Chunk airChunk = new Chunk();

    public int RenderIndex;

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
        return x + (y << SHIFT_Y) + (z << SHIFT_Z);
    }
}
