using UnityEngine;
using System;
using System.Collections.Generic;

public class BlockRendererFactory : MonoBehaviour
{
    Dictionary<string, Func<string[], IBlockRenderer>> m_rendererLoaders = new Dictionary<string, Func<string[], IBlockRenderer>>();

    public IBlockRenderer[] LoadBlockRenderers()
    {
        m_rendererLoaders["DefualtBlockRenderer"] = LoadDefualtRenderer;

        BlockData[] blocks = GetComponent<BlockManager>().Blocks;
        IBlockRenderer[] result = new IBlockRenderer[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            if (string.IsNullOrEmpty(blocks[i].renderer))
                continue;
            result[i] = m_rendererLoaders[blocks[i].renderer](blocks[i].texture.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }
        return result;
    }

    public BlockBounds?[] LoadBlockBounds()
    {
        BlockData[] blocks = GetComponent<BlockManager>().Blocks;
        BlockBounds?[] result = new BlockBounds?[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            if (string.IsNullOrEmpty(blocks[i].bounds))
                continue;
            result[i] = LoadBounds(blocks[i].bounds);
        }
        return result;
    }

    private BlockBounds? LoadBounds(string str)
    {
        if (str == "defualt")
            return new BlockBounds { isDefualt = true };
        var strs = str.Split(' ');
        if (strs.Length == 6)
        {
            Vector3 center = new Vector3();
            Vector3 size = new Vector3();
            for (int i = 0; i < 3; i++)
            {
                center[i] = float.Parse(strs[i]);
                size[i] = float.Parse(strs[i + 3]);
            }
            return new BlockBounds { isDefualt = false, bounds = new Bounds(center, size) };
        }
        Debug.LogError("wrong argument format for block bounds");
        return null;
    }

    private IBlockRenderer LoadDefualtRenderer(string[] args)
    {
        var texManager = GetComponent<BlockTextureManager>();
        if (args.Length == 1)
        {
            Rect uv = texManager.FindBlockTexture(args[0]);
            return new DefualtBlockRenderer(new Rect[] { uv, uv, uv, uv, uv, uv });
        }
        if (args.Length == 6)
        {
            var rects = new Rect[6];
            for (int i = 0; i < 6; i++)
            {
                rects[i] = texManager.FindBlockTexture(args[i]);
            }
            return new DefualtBlockRenderer(rects);
        }
        Debug.LogError("wrong argument format for defualt block renderer");
        Rect e = texManager.FindBlockTexture("Error");
        return new DefualtBlockRenderer(new Rect[] { e, e, e, e, e, e });
    }
}
