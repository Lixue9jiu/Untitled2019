using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CullingManager : MonoBehaviour
{
    Bounds chunkBounds = new Bounds(Vector3.zero, new Vector3(16, 16, 16));

    TerrainManager terrain;
    HashSet<Vector3Int> visitedPos = new HashSet<Vector3Int>();
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
        visitedPos.Clear();
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
                    var chunk = terrain.GetChunk(pos);
                    if (!output.Contains(chunk.RenderIndex))
                    {
                        visitedPos.Add(pos);
                        if (IsFacingView(GetChunkFacePos(i, pos), CellFace.FACES[CellFace.OPPOSITE[i]], position) && (task.faceFrom == -1 || task.chunk.AreFacesConnected(task.faceFrom, i)))
                        {
                            if (FrustumCull(pos, frustum))
                            {
                                tasks.Push(new ChunkTaskInfo { chunk = chunk, faceFrom = CellFace.OPPOSITE[i], pos = pos });
                            }
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
        chunkBounds.center = new Vector3(pos.x * 16 + 8, pos.y * 16 + 8, pos.z * 16 + 8);
        return GeometryUtility.TestPlanesAABB(frustum, chunkBounds);
    }

    struct ChunkTaskInfo
    {
        public Vector3Int pos;
        public int faceFrom;
        public Chunk chunk;
    }
}