using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    Dictionary<Vector3Int, Chunk> m_chunks = new Dictionary<Vector3Int, Chunk>();
    Dictionary<Vector2Int, int> m_yChunkCount = new Dictionary<Vector2Int, int>();

    public Chunk GetChunk(int x, int y, int z)
    {
        if (!m_chunks.TryGetValue(new Vector3Int(x, y, z), out Chunk chunk))
        {
            return Chunk.airChunk;
        }
        return chunk;
    }

    public ChunkStack GetChunkStack(int x, int z)
    {
        int count = m_yChunkCount[new Vector2Int(x, z)];
        Chunk[] chunks = new Chunk[count];
        for (int i = 0; i < count; i++)
        {
            chunks[i] = GetChunk(x, i, z);
        }
        return new ChunkStack(chunks);
    }

    public void SetChunkStack(int x, int z, ChunkStack stack)
    {
        for (int i = 0; i < stack.Length; i++)
        {
            m_chunks[new Vector3Int(x, i, z)] = stack[i];
        }
        m_yChunkCount[new Vector2Int(x, z)] = stack.Length;
    }

    public int GetChunkYCount(int x, int z)
    {
        return m_yChunkCount[new Vector2Int(x, z)];
    }

    public int GetCellValue(int x, int y, int z)
    {
        return GetChunk(x >> Chunk.SHIFT_X, y >> Chunk.SHIFT_Y, z >> Chunk.SHIFT_Z)[x & Chunk.SIZE_X_MINUS_ONE, y & Chunk.SIZE_Y_MINUS_ONE, z & Chunk.SIZE_Z_MINUS_ONE];
    }
}
