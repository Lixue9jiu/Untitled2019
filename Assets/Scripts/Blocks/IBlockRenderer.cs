using UnityEngine;

public interface IBlockRenderer
{
    void DrawTerrain(BlockRenderContext context, Vector3Int pos, IMeshBuilder meshBuilder);
    void DrawStandalone(int value, Matrix4x4 transform, Material sharedMaterial, BlockTextureManager textureManager, int layer, Camera camera);
}
