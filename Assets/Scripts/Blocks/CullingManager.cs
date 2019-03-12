using UnityEngine;
using System.Collections.Generic;

public class CullingManager : MonoBehaviour
{
    TerrainManager terrain;
    Stack<ChunkTaskInfo> tasks = new Stack<ChunkTaskInfo>();

    private void Awake()
    {
        terrain = GetComponent<TerrainManager>();
    }

    public void NoCullingTest(HashSet<int> output)
    {
        foreach (Vector3Int p in terrain.ChunksPos)
        {
            var chunk = terrain.GetChunk(p);
            if (chunk.RenderIndex != -1)
                output.Add(chunk.RenderIndex);
        }
    }

    /// <summary>
    /// look for visible chunks for the camera, the result will be export to the hashset
    /// </summary>
    /// <param name="c">C.</param>
    /// <param name="output">Output.</param>
    public void SearchForVisible(Camera c, HashSet<int> output)
    {
        var frustum = GeometryUtility.CalculateFrustumPlanes(c);
        var forward = c.transform.forward;

        var origin = Vector3Int.FloorToInt(c.transform.position / 16);
        var originChunk = terrain.GetChunk(origin);
        if (originChunk.RenderIndex != -1)
            output.Add(originChunk.RenderIndex);

        for (int i = 0; i < 6; i++)
        {
            var pos = origin + CellFace.FACES[i];
            if (FrustumCull(pos, frustum))
                tasks.Push(new ChunkTaskInfo { chunk = terrain.GetChunk(pos), faceFrom = CellFace.OPPOSITE[i], dirctFrom = CellFace.FACES[i], pos = pos});
        }

        while (tasks.Count > 0)
        {
            var task = tasks.Pop();

            if (task.chunk.RenderIndex != -1)
            {
                output.Add(task.chunk.RenderIndex);
                for (int i = 0; i < 6; i++)
                {
                    var pos = task.pos + CellFace.FACES[i];
                    if (terrain.ChunkExist(pos))
                    {
                        var chunk = terrain.GetChunkFast(pos);
                        if (!output.Contains(chunk.RenderIndex) && task.chunk.AreFacesConnected(task.faceFrom, i) && Vector3.Dot(CellFace.FACES[i], task.dirctFrom) >= 0)
                        {
                            if (FrustumCull(pos, frustum))
                                tasks.Push(new ChunkTaskInfo { chunk = chunk, faceFrom = CellFace.OPPOSITE[i], dirctFrom = pos - origin, pos = pos });
                        }
                    }
                }
            }
        }
    }

    private bool FrustumCull(Vector3Int pos, Plane[] frustum)
    {
        return GeometryUtility.TestPlanesAABB(frustum, new Bounds(pos * 16 + new Vector3Int(8, 8, 8), new Vector3Int(16, 16, 16)));
    }

    struct ChunkTaskInfo
    {
        public Vector3 dirctFrom;
        public int faceFrom;
        public Vector3Int pos;
        public Chunk chunk;
    }
}