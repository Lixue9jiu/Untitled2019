using UnityEngine;
using System.Collections.Generic;

public class CullingManager : MonoBehaviour
{
    TerrainManager terrain;

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
    /// look for visible chunks
    /// </summary>
    /// <param name="c">C.</param>
    /// <param name="output">Output.</param>
    public void SearchForVisible(Camera c, HashSet<int> output)
    {
        var frustum = GeometryUtility.CalculateFrustumPlanes(c);

        var origin = Vector3Int.FloorToInt(c.transform.position / 16);
        var originChunk = terrain.GetChunk(origin);
        if (originChunk.RenderIndex != -1)
            output.Add(originChunk.RenderIndex);

        Stack<ChunkTaskInfo> tasks = new Stack<ChunkTaskInfo>();
        for (int i = 0; i < 6; i++)
        {
            var pos = origin + CellFace.FACES[i];
            tasks.Push(new ChunkTaskInfo { chunk = terrain.GetChunk(pos), faceFrom = CellFace.OPPOSITE[i], pos = pos});
        }

        while (tasks.Count > 0)
        {
            var task = tasks.Pop();
            if (!FrustumCull(task, frustum))
                continue;

            if (task.chunk.RenderIndex != -1)
            {
                if (!output.Add(task.chunk.RenderIndex))
                    continue;
                for (int i = 0; i < 6; i++)
                {
                    if (IsVisible(task, i))
                    {
                        var pos = task.pos + CellFace.FACES[i];
                        tasks.Push(new ChunkTaskInfo { chunk = terrain.GetChunk(pos), faceFrom = CellFace.OPPOSITE[i], pos = pos });
                    }
                }
            }
        }
    }

    private bool IsVisible(ChunkTaskInfo info, int nextFace)
    {
        return nextFace != info.faceFrom && info.chunk.AreFacesConnected(info.faceFrom, nextFace);
    }

    private bool FrustumCull(ChunkTaskInfo info, Plane[] frustum)
    {
        return GeometryUtility.TestPlanesAABB(frustum, new Bounds(info.pos * 16 + new Vector3Int(8, 8, 8), new Vector3Int(16, 16, 16)));
    }

    private bool ViewTest(Vector3 v)
    {
        return v.x > -1 && v.x < 1 && v.y > -1 && v.y < 1;
    }

    class ChunkTaskInfo
    {
        public int faceFrom;
        public Vector3Int pos;
        public Chunk chunk;
    }
}