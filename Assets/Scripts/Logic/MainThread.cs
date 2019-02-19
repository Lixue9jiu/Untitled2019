using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThread : MonoBehaviour
{
    private void Start()
    {
        Test0();
    }

    private void Test0()
    {
        Chunk chunk = new Chunk();
        var block = GetComponent<BlockManager>();
        var bedrock = block.FindBlock("game:bedrock");
        var dirt = block.FindBlock("game:dirt");
        var grass = block.FindBlock("game:grass");
        for (int x = 0; x < Chunk.SIZE_X; x++)
            for (int y = 0; y < Chunk.SIZE_Y; y++)
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    if (y < 2)
                        chunk[x, y, z] = bedrock;
                    else if (y < 9)
                        chunk[x, y, z] = dirt;
                    else if (y < 10)
                        chunk[x, y, z] = grass;
                }

        GetComponent<TerrainManager>().SetChunk(0, 0, chunk);
        GetComponent<TerrainRenderer>().AddChunkToRender(0, 0);
    }

    private void Test1()
    {
        for (int i = 0; i < 16; i++)
        {
            int cx = i & 3;
            int cz = i >> 2;
            var chunk = new Chunk();
            GenerateTerrain(cx << 4, cz << 4, chunk);
            GetComponent<TerrainManager>().SetChunk(cx, cz, chunk);
        }
        for (int i = 0; i < 16; i++)
        {
            GetComponent<TerrainRenderer>().AddChunkToRender(i & 3, i >> 2);
        }
    }

    private void GenerateTerrain(int ox, int oz, Chunk chunk)
    {
        var block = GetComponent<BlockManager>();
        var bedrock = block.FindBlock("game:bedrock");
        var dirt = block.FindBlock("game:dirt");
        var grass = block.FindBlock("game:grass");
        for (int x = 0; x < Chunk.SIZE_X; x++)
            for (int y = 0; y < Chunk.SIZE_Y; y++)
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    float height = Mathf.PerlinNoise((ox + x) / 16f, (oz + z) / 16f) * 20;
                    if (y < 2)
                        chunk[x, y, z] = bedrock;
                    else if (y < height)
                        chunk[x, y, z] = dirt;
                    else if (y < height + 1)
                        chunk[x, y, z] = grass;
                }
    }
}
