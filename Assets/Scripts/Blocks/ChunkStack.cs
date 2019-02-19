using UnityEngine;
using System.Collections;

public class ChunkStack
{
    Chunk[] m_chunks;

    public ChunkStack(int size)
    {
        m_chunks = new Chunk[size];
        for (int i = 0; i < size; i++)
        {
            m_chunks[i] = new Chunk();
        }
    }

    public ChunkStack(Chunk[] chunks)
    {
        m_chunks = chunks;
    }

    public int Length
    {
        get
        {
            return m_chunks.Length;
        }
    }

    public Chunk this[int index]
    {
        get
        {
            return m_chunks[index];
        }
    }

    public int this[int x, int y, int z]
    {
        get
        {
            return m_chunks[y >> Chunk.SHIFT_Y][x, y & Chunk.SIZE_Y_MINUS_ONE, z];
        }
        set
        { 
            m_chunks[y >> Chunk.SHIFT_Y][x, y & Chunk.SIZE_Y_MINUS_ONE, z] = value;
        }
    }
}
