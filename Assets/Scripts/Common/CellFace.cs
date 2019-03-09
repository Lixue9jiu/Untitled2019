using UnityEngine;
using System.Collections;

public static class CellFace
{
    public static readonly Vector3Int[] FACES =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1)
    };

    public static readonly int[] OPPOSITE =
    {
        1,
        0,
        3,
        2,
        5,
        4
    };

    public static readonly Vector3[] DIRECTION =
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(-2, 0, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(-1, 0, -1),
        new Vector3Int(2, 0, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 0, -1),
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, -2, 0),
        new Vector3Int(0, -1, 1),
        new Vector3Int(0, -1, -1),
        new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 2, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 1, 1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, -1),
        new Vector3Int(0, 1, -1),
        new Vector3Int(0, -1, -1),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 0, -2),
        new Vector3Int(1, 0, 1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(0, 1, 1),
        new Vector3Int(0, -1, 1),
        new Vector3Int(0, 0, 2),
        new Vector3Int(0, 0, 0)
    };

    static CellFace()
    {
        for (int i = 0; i < DIRECTION.Length; i++)
        {
            DIRECTION[i].Normalize();
        }
    }

    public static Vector3 GetDirection(int from, int to)
    {
        return DIRECTION[from * 6 + to];
    }
}
