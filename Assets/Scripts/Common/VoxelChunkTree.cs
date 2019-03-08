using UnityEngine;
using System.Collections.Generic;

public class VoxelChunkTree
{
    public class Node
    {
        public readonly Node[] neighbors = new Node[6];
        public readonly Vector3Int coord;
        public readonly Chunk chunk;

        public Node(Vector3Int coord, Chunk chunk)
        {
            this.coord = coord;
            this.chunk = chunk;
        }
    }

    Dictionary<Vector3Int, Node> nodes = new Dictionary<Vector3Int, Node>();

    public void AddChunk(Vector3Int pos, Chunk chunk)
    {
        var n = new Node(pos, chunk);
        for (int i = 0; i < 6; i++)
        {
            var neighbor = pos + CellFace.FACES[i];
            if (nodes.TryGetValue(neighbor, out Node neighborNode))
            {
                neighborNode.neighbors[CellFace.OPPOSITE[i]] = n;
                n.neighbors[i] = neighborNode;
            }
        }
        nodes[pos] = n;
    }

    public void RemoveChunk(Vector3Int pos)
    {
        var node = nodes[pos];
        for (int i = 0; i < 6; i++)
        {
            if (node.neighbors[i] != null)
            {
                node.neighbors[i].neighbors[CellFace.OPPOSITE[i]] = null;
            }
        }
        nodes[pos] = null;
    }

    public bool Contains(Vector3Int pos)
    {
        return nodes.ContainsKey(pos);
    }

    public Node this[Vector3Int pos] => nodes[pos];
    public Node this[int x, int y, int z] => this[new Vector3Int(x, y, z)];
}
