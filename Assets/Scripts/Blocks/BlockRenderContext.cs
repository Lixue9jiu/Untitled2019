using UnityEngine;
using System.Collections;

public class BlockRenderContext
{
    /// <summary>
    /// The chunk containing the block
    /// </summary>
    public Chunk chunk;
    /// <summary>
    /// The neighbor chunks in the order of x+, x-, z+, z-
    /// </summary>
    public Chunk[] neighbors;
    /// <summary>
    /// The block manager.
    /// </summary>
    public BlockManager blockManager;
}
