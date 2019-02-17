using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    public Chunk GetChunk(int x, int z)
    {
        if (!chunks.TryGetValue(new Vector2Int(x, z), out Chunk chunk))
        {
            return Chunk.airChunk;
        }
        return chunk;
    }

    public void SetChunk(int x, int z, Chunk chunk)
    {
        chunks[new Vector2Int(x, z)] = chunk;
    }
}
