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

    public bool IsEmpty;

    bool[] opaqueMask = new bool[36];

    public bool AreFacesConnected(int a, int b)
    {
        return opaqueMask[a * 6 + b];
    }

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

    public void RecalculateOpaques(BlockManager manager)
    {
        bool[] result = new bool[36];
        bool[,,] mask = new bool[SIZE_X, SIZE_Y, SIZE_Z];
        for (int x = 0; x < SIZE_X; x++)
            for (int y = 0; y < SIZE_Y; y++)
                for (int z = 0; z < SIZE_Z; z++)
                {
                    if (!mask[x, y, z] && !manager.IsOpaque(this[x, y, z]))
                    {
                        bool[] faceFlag = new bool[6];
                        FloodFill(x, y, z, ref faceFlag, mask, manager);

                        for (int i = 0; i < 6; i++)
                            if (faceFlag[i])
                                for (int k = i; k < 6; k++)
                                    if (faceFlag[k])
                                    {
                                        result[k * 6 + i] = true;
                                        result[i * 6 + k] = true;
                                    }
                    }
                }
        opaqueMask = result;
    }

    // cumulative value:
    // 1-2: 1
    // 1-3: 2
    // 1-4: 4
    // 1-5: 8
    // 1-6: 16
    // 2-3: 32
    // 2-4: 64
    // 2-5: 128
    // 2-6: 256
    // 3-4: 512
    // 3-5: 1024
    // 3-6: 2048
    // 4-5: 4096
    // 4-6: 8192
    // 5-6: 16384
    private void FloodFill(int x, int y, int z, ref bool[] cumulative, bool[,,] mask, BlockManager manager)
    {
        if (x < 0 || x == SIZE_X || y < 0 || y == SIZE_Y || z < 0 || z == SIZE_Z)
            return;
        if (!mask[x, y, z] && !manager.IsOpaque(this[x, y, z]))
        {
            if (x == 0)
                cumulative[1] = true;
            else if (x == SIZE_X_MINUS_ONE)
                cumulative[0] = true;
            if (y == 0)
                cumulative[3] = true;
            else if (y == SIZE_Y_MINUS_ONE)
                cumulative[2] = true;
            if (z == 0)
                cumulative[5] = true;
            else if (z == SIZE_Z_MINUS_ONE)
                cumulative[4] = true;

            mask[x, y, z] = true;

            FloodFill(x + 1, y + 1, z + 1, ref cumulative, mask, manager);
            FloodFill(x - 1, y + 1, z + 1, ref cumulative, mask, manager);
            FloodFill(x + 1, y - 1, z + 1, ref cumulative, mask, manager);
            FloodFill(x - 1, y - 1, z + 1, ref cumulative, mask, manager);
            FloodFill(x + 1, y + 1, z - 1, ref cumulative, mask, manager);
            FloodFill(x - 1, y + 1, z - 1, ref cumulative, mask, manager);
            FloodFill(x + 1, y - 1, z - 1, ref cumulative, mask, manager);
            FloodFill(x - 1, y - 1, z - 1, ref cumulative, mask, manager);
        }
    }
}
