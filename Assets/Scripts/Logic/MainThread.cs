﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThread : MonoBehaviour
{
    [SerializeField]
    FirstPersonController player;

    [SerializeField]
    GameObject inventoryPrefab;

    private void Start()
    {
        Test2();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GetComponent<WindowManager>().Escape())
            player.enabled = !player.enabled;
        if (CrossPlatfromInput.instance.GetButtonDown("Inventory") && !GetComponent<WindowManager>().IsShowing<CreativeItemMenu>())
        {
            GetComponent<WindowManager>().AddWindow(inventoryPrefab);
        }
    }

    private void Test0()
    {
        ChunkStack chunk = new ChunkStack(1);
        var block = GetComponent<BlockManager>();
        var bedrock = block.FindBlock("game:bedrock");
        var dirt = block.FindBlock("game:dirt");
        var grass = block.FindBlock("game:grass");
        var stone = block.FindBlock("game:stone");
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
        //for (int x = 6; x < 10; x++)
        //for (int z = 6; z < 10; z++)
        //for (int y = 10; y < 14; y++)
        //chunk[x, y, z] = stone;

        GetComponent<TerrainManager>().SetChunkStack(0, 0, chunk);
        GetComponent<TerrainRenderer>().AddChunkStackToRender(0, 0);

        for (int i = 0; i < 6; i++)
            for (int k = 0; k < 6; k++)
            {
                Debug.Log($"faces {i} and {k} connected: {chunk[0].AreFacesConnected(i, k)}");
            }
    }

    private void Test1()
    {
        for (int i = 0; i < 16; i++)
        {
            int cx = i & 3;
            int cz = i >> 2;
            var chunk = new ChunkStack(4);
            GenerateTerrain(cx << 4, cz << 4, chunk);
            GetComponent<TerrainManager>().SetChunkStack(cx, cz, chunk);
        }
        for (int i = 0; i < 16; i++)
        {
            GetComponent<TerrainRenderer>().AddChunkStackToRender(i & 3, i >> 2);
        }
    }

    private void Test2()
    {
        for (int i = 0; i < 256; i++)
        {
            int cx = i & 15;
            int cz = i >> 4;
            var chunk = new ChunkStack(4);
            GenerateTerrain(cx << 4, cz << 4, chunk);
            GetComponent<TerrainManager>().SetChunkStack(cx, cz, chunk);
        }
        for (int i = 0; i < 256; i++)
        {
            GetComponent<TerrainRenderer>().AddChunkStackToRender(i & 15, i >> 4);
        }
    }

    private void GenerateTerrain(int ox, int oz, ChunkStack chunk)
    {
        var block = GetComponent<BlockManager>();
        var bedrock = block.FindBlock("game:bedrock");
        var dirt = block.FindBlock("game:dirt");
        var grass = block.FindBlock("game:grass");
        for (int x = 0; x < Chunk.SIZE_X; x++)
            for (int y = 0; y < Chunk.SIZE_Y * 4; y++)
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    float height = GenerateHeight(ox + x, oz + z);
                    if (y < 2)
                        chunk[x, y, z] = bedrock;
                    else if (y < height)
                        chunk[x, y, z] = dirt;
                    else if (y < height + 1)
                        chunk[x, y, z] = grass;
                }
    }

    private void GenerateTerrainFlat(int ox, int oz, ChunkStack chunk)
    {
        var block = GetComponent<BlockManager>();
        var bedrock = block.FindBlock("game:bedrock");
        var dirt = block.FindBlock("game:dirt");
        var grass = block.FindBlock("game:grass");
        for (int x = 0; x < Chunk.SIZE_X; x++)
            for (int y = 0; y < Chunk.SIZE_Y * 4; y++)
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    if (y < 2)
                        chunk[x, y, z] = bedrock;
                    else if (y < 10)
                        chunk[x, y, z] = dirt;
                    else if (y < 11)
                        chunk[x, y, z] = grass;
                }
    }

    private void GenerateTerrain3d(int ox, int oz, ChunkStack chunk)
    {
        float cutoff = 0.4f;
        var block = GetComponent<BlockManager>();
        var stone = block.FindBlock("game:stone");
        for (int x = 0; x < Chunk.SIZE_X; x ++)
            for (int y = 0; y < Chunk.SIZE_Y * 4; y++)
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    float sx = ox + x;
                    float sy = y;
                    float sz = oz + z;

                    float value = (PerlinNoise(sx / 64f, sy / 64f, sz / 64f) +
                        PerlinNoise(sx / 32f, sy / 32f, sz / 64f) * 0.5f +
                        PerlinNoise(sx / 16f, sy / 16f, sz / 64f) * 0.25f) / 1.75f;

                    if (value > cutoff)
                        chunk[x, y, z] = stone;
                }
    }

    private float PerlinNoise(float sx, float sy, float sz)
    {
        float AB = Mathf.PerlinNoise(sx, sy);
        float BC = Mathf.PerlinNoise(sy, sz);
        float AC = Mathf.PerlinNoise(sx, sz);

        float BA = Mathf.PerlinNoise(sy, sx);
        float CB = Mathf.PerlinNoise(sz, sy);
        float CA = Mathf.PerlinNoise(sz, sx);

        return (AB + BA + BC + CB + AC + CA) / 6f;
    }

    private float GenerateHeight(int x, int y)
    {
        //return Mathf.PerlinNoise(x / 32f, y / 32f) +
        //Mathf.PerlinNoise(x / 24f, y / 24f) * 5 +
        //Mathf.PerlinNoise(x / 16f, y / 16f) * 10 + 
        //Mathf.PerlinNoise(x / 8f, y / 8f) * 30;
        return Mathf.Pow(Mathf.PerlinNoise(x / 96f, y / 96f), 4f) * 48 +
            Mathf.PerlinNoise(x / 64f, y / 64f) * 24 +
            Mathf.PerlinNoise(x / 32f, y / 32f) * 12 +
            Mathf.PerlinNoise(x / 16f, y / 16f) * 6;
    }
}
