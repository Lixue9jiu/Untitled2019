using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TerrainRenderer : MonoBehaviour
{
    TerrainManager m_terrainManager;
    BlockManager m_blockManager;

    IBlockRenderer[] m_renderers;
    BlockBounds?[] m_bounds;

    List<ChunkInstance> instances = new List<ChunkInstance>();
    Queue<int> freeIndices = new Queue<int>();

    [SerializeField]
    Material opaueMaterial;

    [SerializeField]
    GameObject terrian;

    [SerializeField]
    GameObject chunkPrefab;

    class ChunkInstance
    {
        public Mesh mesh;
        public Matrix4x4 matrix;
        public MeshCollider collider;
    }

    private void Awake()
    {
        m_terrainManager = GetComponent<TerrainManager>();
        m_blockManager = GetComponent<BlockManager>();
    }

    private void Start()
    {
        m_renderers = GetComponent<BlockRendererFactory>().LoadBlockRenderers();
        m_bounds = GetComponent<BlockRendererFactory>().LoadBlockBounds();
        opaueMaterial.mainTexture = GetComponent<BlockTextureManager>().MainTexture;
    }

    private void Update()
    {
        for (int i = 0; i < instances.Count; i++)
        {
            if (instances[i] != null)
                Graphics.DrawMesh(instances[i].mesh, instances[i].matrix, opaueMaterial, 0);
        }
    }

    public void AddChunkToRender(int x, int z)
    {
        var instance = new ChunkInstance { mesh = BuildMesh(x, z), matrix = Matrix4x4.Translate(new Vector3(x << 4, 0, z << 4)) };
        instance.collider = Instantiate(chunkPrefab, terrian.transform).GetComponent<MeshCollider>();
        instance.collider.sharedMesh = BuildBoundingMesh(x, z);
        instance.collider.transform.position = new Vector3(x << 4, 0, z << 4);
        if (freeIndices.Count == 0)
        {
            m_terrainManager.GetChunk(x, z).RenderIndex = instances.Count;
            instances.Add(instance);
        }
        else
        {
            int index = freeIndices.Dequeue();
            m_terrainManager.GetChunk(x, z).RenderIndex = index;
            instances[index] = instance;
        }
    }

    public void RemoveChunkFromRender(int x, int z)
    {
        int index = m_terrainManager.GetChunk(x, z).RenderIndex;
        Destroy(instances[index].collider);
        instances[index] = null;
        freeIndices.Enqueue(index);
    }

    private Mesh BuildBoundingMesh(int cx, int cz)
    {
        var chunk = m_terrainManager.GetChunk(cx, cz);
        Chunk[] neighbors =
        {
            m_terrainManager.GetChunk(cx + 1, cz),
            m_terrainManager.GetChunk(cx - 1, cz),
            m_terrainManager.GetChunk(cx, cz + 1),
            m_terrainManager.GetChunk(cx, cz - 1)
        };
        var render = new DefualtBlockRenderer(new Rect[6]);
        MeshBuilder builder = new MeshBuilder();
        for (int x = 0; x < Chunk.SIZE_X; x++)
        {
            for (int y = 0; y < Chunk.SIZE_Y; y++)
            {
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    var bb = m_bounds[BlockData.GetContent(chunk[x, y, z])];
                    if (bb.HasValue)
                        render.Render(new BlockRenderContext { chunk = chunk, neighbors = neighbors, blockManager = m_blockManager }, new Vector3Int(x, y, z), builder);
                }
            }
        }
        return builder.ToMesh();
    }

    private Mesh BuildMesh(int cx, int cz)
    {
        var chunk = m_terrainManager.GetChunk(cx, cz);
        Chunk[] neighbors =
        {
            m_terrainManager.GetChunk(cx + 1, cz),
            m_terrainManager.GetChunk(cx - 1, cz),
            m_terrainManager.GetChunk(cx, cz + 1),
            m_terrainManager.GetChunk(cx, cz - 1)
        };
        MeshBuilder builder = new MeshBuilder();
        for (int x = 0; x < Chunk.SIZE_X; x++)
        {
            for (int y = 0; y < Chunk.SIZE_Y; y++)
            {
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    m_renderers[BlockData.GetContent(chunk[x, y, z])]?.Render(new BlockRenderContext { chunk = chunk, neighbors = neighbors, blockManager = m_blockManager }, new Vector3Int(x, y, z), builder);
                }
            }
        }
        return builder.ToMesh();
    }
}
