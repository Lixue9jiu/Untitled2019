using UnityEngine;

public class DefualtBlockRenderer : IBlockRenderer
{
    private readonly bool[] m_useRandomVert;
    private readonly Rect[] m_uvs;

    private readonly Mesh standaloneMesh;

    public DefualtBlockRenderer(Rect[] uvs, bool[] useRandomVert)
    {
        m_uvs = uvs;
        m_useRandomVert = useRandomVert;

        MeshBuilder builder = new MeshBuilder();
        for (int i = 0; i < 6; i++)
        {
            var faces = BlockRenderUtils.faces[i];
            var color = BlockRenderUtils.CalculateShadow(BlockRenderUtils.mainLight, BlockRenderUtils.offsets[i]);
            builder.Quad(faces[0], faces[1], faces[2], faces[3], uvs[i], color, color, color, color, MeshBuilderFlags.None);
        }
        standaloneMesh = builder.ToMesh();
    }

    public void DrawTerrain(BlockRenderContext context, Vector3Int pos, IMeshBuilder meshBuilder)
    {
        int value = context.chunk[pos];
        int[] neighbors = GetNeighborsFast(context, pos);

        for (int i = 0; i < 6; i++)
        {
            if (value != neighbors[i] && BlockRenderUtils.IsTransparent(context, neighbors[i]))
            {
                BlockRenderUtils.ShadedQuad(context, meshBuilder, pos, i, m_uvs[i], Color.white, Color.white, Color.white, Color.white, m_useRandomVert[i]);
            }
        }
    }

    private static int[] GetNeighborsFast(BlockRenderContext context, Vector3Int pos)
    {
        return new int[] {
            pos.x == Chunk.SIZE_X_MINUS_ONE ? context.neighbors[0][0, pos.y, pos.z] : context.chunk[pos.x + 1, pos.y, pos.z],
            pos.x == 0 ? context.neighbors[1][Chunk.SIZE_X_MINUS_ONE, pos.y, pos.z] : context.chunk[pos.x - 1, pos.y, pos.z],
            pos.y == Chunk.SIZE_Y_MINUS_ONE ? context.neighbors[2][pos.x, 0, pos.z] : context.chunk[pos.x, pos.y + 1, pos.z],
            pos.y == 0 ? context.neighbors[3][pos.x, Chunk.SIZE_Y_MINUS_ONE, pos.z] : context.chunk[pos.x, pos.y - 1, pos.z],
            pos.z == Chunk.SIZE_Z_MINUS_ONE ? context.neighbors[4][pos.x, pos.y, 0] : context.chunk[pos.x, pos.y, pos.z + 1],
            pos.z == 0 ? context.neighbors[5][pos.x, pos.y, Chunk.SIZE_Z_MINUS_ONE] : context.chunk[pos.x, pos.y, pos.z - 1]
        };
    }

    public void DrawStandalone(int value, Matrix4x4 transform, Material sharedMaterial, BlockTextureManager textureManager, int layer, Camera camera)
    {
        sharedMaterial.mainTexture = textureManager.MainTexture;
        Graphics.DrawMesh(standaloneMesh, transform, sharedMaterial, layer, camera);
    }
}
