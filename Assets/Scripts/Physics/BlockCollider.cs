using UnityEngine;
using System.Collections.Generic;

public class BlockCollider : MonoBehaviour
{
    [SerializeField]
    GameObject game;
    TerrainManager terrain;
    TerrainRenderer terrainRenderer;

    Dictionary<Vector3Int, BoxCollider> colliders = new Dictionary<Vector3Int, BoxCollider>();

    private void Awake()
    {
        terrain = game.GetComponent<TerrainManager>();
        terrainRenderer = game.GetComponent<TerrainRenderer>();
    }

    public void AddBlockCollider(Vector3Int pos)
    {
        var b = terrainRenderer.BlockBounds[BlockData.GetContent(terrain.GetCellValue(pos))];
        if (b.HasValue && !colliders.ContainsKey(pos))
        {
            var c = gameObject.AddComponent<BoxCollider>();
            c.center = pos + b.Value.bounds.center;
            c.size = b.Value.bounds.size;
            colliders[pos] = c;
        }
    }

    public void ClearColliders()
    {
        colliders.Clear();
        var boxes = GetComponents<BoxCollider>();
        foreach (BoxCollider b in boxes)
        {
            Destroy(b);
        }
    }
}
