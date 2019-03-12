using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

public class TerrainRenderer : MonoBehaviour
{
    TerrainManager m_terrainManager;
    BlockManager m_blockManager;

    IBlockRenderer[] m_renderers;
    BlockBounds?[] m_bounds;

    List<ChunkInstance> instances = new List<ChunkInstance>();
    Queue<int> freeIndices = new Queue<int>();

    HashSet<int> visibleChunks = new HashSet<int>();

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
        visibleChunks.Clear();
        GetComponent<CullingManager>().SearchForVisible(Camera.main, visibleChunks);
        //GetComponent<CullingManager>().NoCullingTest(visibleChunks);
        GetComponent<LabelRenderer>().AddLabel($"chunks rendered: {visibleChunks.Count}");
        foreach (int i in visibleChunks)
        {
            Graphics.DrawMesh(instances[i].mesh, instances[i].matrix, opaueMaterial, 0);
        }
    }

    public void AddChunkStackToRender(int x, int z)
    {
        int count = m_terrainManager.GetChunkYCount(x, z);
        for (int i = 0; i < count; i++)
        {
            AddChunkToRender(x, i, z);
        }
    }

    public void RemoveChunkStackFromRender(int x, int z)
    {
        int count = m_terrainManager.GetChunkYCount(x, z);
        for (int i = 0; i < count; i++)
        {
            RemoveChunkFromRender(x, i, z);
        }
    }

    public void AddChunkToRender(int x, int y, int z)
    {
        var instance = new ChunkInstance { matrix = Matrix4x4.Translate(new Vector3(x << Chunk.SHIFT_X, y << Chunk.SHIFT_Y, z << Chunk.SHIFT_Z)) };
        instance.collider = Instantiate(chunkPrefab, terrian.transform).GetComponent<MeshCollider>();
        instance.collider.transform.position = new Vector3(x << Chunk.SHIFT_X, y << Chunk.SHIFT_Y, z << Chunk.SHIFT_Z);
        if (freeIndices.Count == 0)
        {
            m_terrainManager.GetChunk(x, y, z).RenderIndex = instances.Count;
            instances.Add(instance);
        }
        else
        {
            int index = freeIndices.Dequeue();
            m_terrainManager.GetChunk(x, y, z).RenderIndex = index;
            instances[index] = instance;
        }
        QueueChunkUpdate(x, y, z);
    }

    public void RemoveChunkFromRender(int x, int y, int z)
    {
        RemoveChunkFromRender(ref m_terrainManager.GetChunk(x, y, z).RenderIndex);
    }

    public void RemoveChunkFromRender(ref int index)
    {
        if (index == -1)
            return;
        Destroy(instances[index].collider.gameObject);
        instances[index] = null;
        freeIndices.Enqueue(index);
        index = -1;
    }

    public void QueueChunkUpdate(int x, int y, int z)
    {
        int index = m_terrainManager.GetChunk(x, y, z).RenderIndex;
        if (index == -1)
            return;

        var mb = new MeshBuilder();
        var cb = new ColliderMeshBuilder();
        GetComponent<TaskManager>().CreateWork(UpdateChunk, new object[] { new Vector3Int(x, y, z), mb, cb }, () =>
        {
            instances[index].mesh = mb.ToMesh();
            instances[index].collider.sharedMesh = cb.ToMesh();
            mb.Clear();
            cb.Clear();
        });
    }

    public void UpdateChunk(object obj)
    {
        var data = (object[])obj;
        var p = (Vector3Int)data[0];
        BuildMesh((MeshBuilder)data[1], p.x, p.y, p.z);
        BuildBoundingMesh((ColliderMeshBuilder)data[2], p.x, p.y, p.z);
    }

    /// <summary>
    /// Builds the bounding mesh using Greedy Meshing
    /// </summary>
    /// <returns>The bounding mesh.</returns>
    /// <param name="cx">chunkx</param>
    /// <param name="cy">chunky</param>
    /// <param name="cz">chunkz</param>
    private void BuildBoundingMesh(ColliderMeshBuilder builder, int cx, int cy, int cz)
    {
        var chunk = m_terrainManager.GetChunk(cx, cy, cz);
        Chunk[] neighbors =
        {
            m_terrainManager.GetChunk(cx - 1, cy, cz),
            m_terrainManager.GetChunk(cx, cy - 1, cz),
            m_terrainManager.GetChunk(cx, cy, cz - 1),
            m_terrainManager.GetChunk(cx + 1, cy, cz),
            m_terrainManager.GetChunk(cx, cy + 1, cz),
            m_terrainManager.GetChunk(cx, cy, cz + 1)
        };

        Vector3Int dim = new Vector3Int(Chunk.SIZE_X, Chunk.SIZE_Y, Chunk.SIZE_Z);
        Vector3Int dimShift = new Vector3Int(Chunk.SIZE_X_MINUS_ONE, Chunk.SIZE_Y_MINUS_ONE, Chunk.SIZE_Z_MINUS_ONE);
        for (int d = 0; d < 3; d++)
        {
            int u = (d + 1) % 3;
            int v = (d + 2) % 3;

            int[,] masks = new int[dim[u], dim[v]];

            Vector3Int monika = new Vector3Int();
            monika[d] = 1;
            for (int i = -1; i < dim[d]; i++)
            {
                for (int j = 0; j < dim[u]; j++)
                {
                    for (int k = 0; k < dim[v]; k++)
                    {
                        Vector3Int x = new Vector3Int();
                        x[d] = i;
                        x[u] = j;
                        x[v] = k;

                        BlockBounds? a;
                        BlockBounds? b;
                        if (x[d] == -1)
                            a = m_bounds[BlockData.GetContent(neighbors[d][x.x & dimShift.x, x.y & dimShift.y, x.z & dimShift.z])];
                        else
                            a = m_bounds[BlockData.GetContent(chunk[x])];
                        if (x[d] + 1 == dim[d])
                            b = m_bounds[BlockData.GetContent(neighbors[d + 3][(x.x + monika.x) & dimShift.x, (x.y + monika.y) & dimShift.y, (x.z + monika.z) & dimShift.z])];
                        else
                            b = m_bounds[BlockData.GetContent(chunk[x + monika])];
                        bool hasFace = (a.HasValue && a.Value.isDefualt) != (b.HasValue && b.Value.isDefualt);
                        masks[j, k] = (hasFace ? 1 : 0) + (a.HasValue && a.Value.isDefualt ? 2 : 0);
                    }
                }

                for (int j = 0; j < dim[u]; j++)
                {
                    for (int k = 0; k < dim[v];)
                    {
                        int point = masks[j, k];
                        if ((point & 1) == 0)
                        {
                            k++;
                            continue;
                        }
                        int w = 1;
                        int h = 1;

                        while (k + h < dim[v] && masks[j, k + h] == point)
                        {
                            masks[j, k + h] = 0;
                            h++;
                        }

                        while (j + w < dim[u])
                        {
                            for (int l = 0; l < h; l++)
                            {
                                if (masks[j + w, k + l] != point)
                                    goto here;
                            }
                            for (int l = 0; l < h; l++)
                            {
                                masks[j + w, k + l] = 0;
                            }
                            w++;
                        }
                    here:

                        Vector3Int x = new Vector3Int();
                        x[d] = i + 1;
                        x[u] = j;
                        x[v] = k;
                        Vector3 umax = new Vector3();
                        Vector3 vmax = new Vector3();
                        umax[u] = w;
                        vmax[v] = h;

                        if ((point & 2) == 2)
                            builder.Quad(x, x + umax, x + umax + vmax, x + vmax);
                        else
                            builder.Quad(x, x + vmax, x + umax + vmax, x + umax);

                        k += h;
                    }
                }
            }
        }
    }

    private void BuildMesh(MeshBuilder builder, int cx, int cy, int cz)
    {
        var chunk = m_terrainManager.GetChunk(cx, cy, cz);
        Chunk[] neighbors =
        {
            m_terrainManager.GetChunk(cx + 1, cy, cz),
            m_terrainManager.GetChunk(cx - 1, cy, cz),
            m_terrainManager.GetChunk(cx, cy + 1, cz),
            m_terrainManager.GetChunk(cx, cy - 1, cz),
            m_terrainManager.GetChunk(cx, cy, cz + 1),
            m_terrainManager.GetChunk(cx, cy, cz - 1)
        };
        var origin = new Vector3Int(cx << Chunk.SHIFT_X, cy << Chunk.SHIFT_Y, cz << Chunk.SHIFT_Z);
        for (int x = 0; x < Chunk.SIZE_X; x++)
        {
            for (int y = 0; y < Chunk.SIZE_Y; y++)
            {
                for (int z = 0; z < Chunk.SIZE_Z; z++)
                {
                    var context = new BlockRenderContext
                    {
                        origin = origin,
                        chunk = chunk,
                        neighbors = neighbors,
                        blockManager = m_blockManager,
                        terrainManager = m_terrainManager
                    };
                    m_renderers[BlockData.GetContent(chunk[x, y, z])]?.Render(context, new Vector3Int(x, y, z), builder);
                }
            }
        }
    }
}
