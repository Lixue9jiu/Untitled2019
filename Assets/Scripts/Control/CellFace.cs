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
}
