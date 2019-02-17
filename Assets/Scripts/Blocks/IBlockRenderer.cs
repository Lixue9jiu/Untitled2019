using UnityEngine;

public interface IBlockRenderer
{
    void Render(BlockRenderContext context, Vector3Int pos, IMeshBuilder meshBuilder);
}
