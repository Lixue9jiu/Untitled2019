using UnityEngine;
using System.Collections.Generic;

public class CullingManager : MonoBehaviour
{
    TerrainManager terrain;
    Stack<ChunkTaskInfo> tasks = new Stack<ChunkTaskInfo>();

    struct Line
    {
        public Vector3 from;
        public Vector3 to;
    }

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
        var position = c.transform.position;

        var origin = Vector3Int.FloorToInt(position / 16);
        var originChunk = terrain.GetChunk(origin);

        tasks.Push(new ChunkTaskInfo { chunk = originChunk, faceFrom = -1, pos = origin });

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
                        if (!output.Contains(chunk.RenderIndex) && IsFacingView(GetChunkFacePos(i, pos), CellFace.FACES[CellFace.OPPOSITE[i]], position) && (task.faceFrom == -1 || task.chunk.AreFacesConnected(task.faceFrom, i)))
                        {
                            if (FrustumCull(pos, frustum))
                                tasks.Push(new ChunkTaskInfo { chunk = chunk, faceFrom = CellFace.OPPOSITE[i], pos = pos });
                        }
                    }
                }
            }
        }
    }

    private static readonly Vector3[] ChunkFaces =
    {
        new Vector3(16, 8, 8),
        new Vector3(0, 8, 8),
        new Vector3(8, 16, 8),
        new Vector3(8, 0, 8),
        new Vector3(8, 8, 16),
        new Vector3(8, 8, 0)
    };

    public static Vector3 GetChunkFacePos(int face, Vector3Int chunkPos)
    {
        return chunkPos * 16 + ChunkFaces[face];
    }

    public static bool IsFacingView(Vector3 facePos, Vector3 faceNormal, Vector3 viewPos)
    {
        return Vector3.Dot(faceNormal, facePos - viewPos) < 0;
    }

    private bool FrustumCull(Vector3Int pos, Plane[] frustum)
    {
        return GeometryUtility.TestPlanesAABB(frustum, new Bounds(pos * 16 + new Vector3Int(8, 8, 8), new Vector3Int(16, 16, 16)));
    }

    struct ChunkTaskInfo
    {
        public int faceFrom;
        public Vector3Int pos;
        public Chunk chunk;
    }
}