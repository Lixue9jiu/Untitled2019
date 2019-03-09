using UnityEngine;
using System.Collections.Generic;

public class TerrainManager : MonoBehaviour
{
    Dictionary<Vector3Int, Chunk> m_chunks = new Dictionary<Vector3Int, Chunk>();
    Dictionary<Vector2Int, int> m_yChunkCount = new Dictionary<Vector2Int, int>();

    public ICollection<Vector3Int> ChunksPos => m_chunks.Keys;

    public Chunk GetChunk(Vector3Int pos)
    {
        if (!m_chunks.TryGetValue(pos, out Chunk chunk))
        {
            return Chunk.airChunk;
        }
        return chunk;
    }

    public bool ChunkExist(Vector3Int pos)
    {
        return m_chunks.ContainsKey(pos);
    }

    public Chunk GetChunkFast(Vector3Int pos)
    {
        return m_chunks[pos];
    }

    public Chunk GetChunk(int x, int y, int z)
    {
        return GetChunk(new Vector3Int(x, y, z));
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
            stack[i].RecalculateOpaques(GetComponent<BlockManager>());
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

    public void SetCellValue(int x, int y, int z, int value)
    {
        var point = new Vector3Int(x >> Chunk.SHIFT_X, y >> Chunk.SHIFT_Y, z >> Chunk.SHIFT_Z);
        if (m_chunks.TryGetValue(point, out Chunk c))
        {
            c[x & Chunk.SIZE_X_MINUS_ONE, y & Chunk.SIZE_Y_MINUS_ONE, z & Chunk.SIZE_Z_MINUS_ONE] = value;

            var r = GetComponent<TerrainRenderer>();
            r.QueueChunkUpdate(point.x, point.y, point.z);

            x &= Chunk.SIZE_X_MINUS_ONE;
            y &= Chunk.SIZE_Y_MINUS_ONE;
            z &= Chunk.SIZE_Z_MINUS_ONE;

            if (x == Chunk.SIZE_X_MINUS_ONE)
                r.QueueChunkUpdate(point.x + 1, point.y, point.z);
            else if (x == 0)
                r.QueueChunkUpdate(point.x - 1, point.y, point.z);

            if (y == Chunk.SIZE_Y_MINUS_ONE)
                r.QueueChunkUpdate(point.x, point.y + 1, point.z);
            else if (y == 0)
                r.QueueChunkUpdate(point.x, point.y - 1, point.z);

            if (z == Chunk.SIZE_Z_MINUS_ONE)
                r.QueueChunkUpdate(point.x, point.y, point.z + 1);
            else if (z == 0)
                r.QueueChunkUpdate(point.x, point.y, point.z - 1);
        }
    }
}
