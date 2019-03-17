using UnityEngine;
using System;
using System.Collections.Generic;

public class BlockRendererFactory : MonoBehaviour
{
    Dictionary<string, Func<ValuesDictionary, IBlockRenderer>> m_rendererLoaders = new Dictionary<string, Func<ValuesDictionary, IBlockRenderer>>();

    public IBlockRenderer[] LoadBlockRenderers()
    {
        m_rendererLoaders["DefualtBlockRenderer"] = LoadDefualtRenderer;
        m_rendererLoaders["MeshBlockRenderer"] = LoadMeshBlockRenderer;

        BlockData[] blocks = GetComponent<BlockManager>().Blocks;
        IBlockRenderer[] result = new IBlockRenderer[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            if (string.IsNullOrEmpty(blocks[i].renderer))
                continue;
            result[i] = m_rendererLoaders[blocks[i].renderer](blocks[i].renderInfo);
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

    private IBlockRenderer LoadDefualtRenderer(ValuesDictionary dict)
    {
        var texManager = GetComponent<BlockTextureManager>();
        var args = dict.GetValue<string>("texture").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 1)
        {
            bool useRandom = false;
            if (args[0][args[0].Length - 1] == '*')
            {
                useRandom = true;
                args[0] = args[0].Remove(args[0].Length - 1);
            }
            Rect uv = texManager.FindBlockTexture(args[0]);
            return new DefualtBlockRenderer(new Rect[] { uv, uv, uv, uv, uv, uv }, new bool[] { useRandom, useRandom, useRandom, useRandom, useRandom, useRandom });
        }
        if (args.Length == 6)
        {
            var rects = new Rect[6];
            var useRandom = new bool[6];
            for (int i = 0; i < 6; i++)
            {
                if (args[i][args[i].Length - 1] == '*')
                {
                    useRandom[i] = true;
                    args[i] = args[i].Remove(args[i].Length - 1);
                }
                rects[i] = texManager.FindBlockTexture(args[i]);
            }
            return new DefualtBlockRenderer(rects, useRandom);
        }
        Debug.LogError("wrong argument format for defualt block renderer");
        Rect e = texManager.FindBlockTexture("Error");
        return new DefualtBlockRenderer(new Rect[] { e, e, e, e, e, e }, new bool[] { false, false, false, false, false, false });
    }

    private IBlockRenderer LoadMeshBlockRenderer(ValuesDictionary dict)
    {
        var texManager = GetComponent<BlockTextureManager>();
        var args = dict.GetValue<string>("texture").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (args.Length == 1)
        {
            Rect uv = texManager.FindBlockTexture(args[0]);
            Mesh m = GetComponent<ContentManager>().GetBundle("meshes").LoadAsset<Mesh>(dict.GetValue<string>("mesh"));
            return new MeshBlockRenderer(uv, m);
        }
        Debug.LogError("wrong argument format for mesh block renderer");
        Rect e = texManager.FindBlockTexture("Error");
        return new DefualtBlockRenderer(new Rect[] { e, e, e, e, e, e }, new bool[] { false, false, false, false, false, false });
    }
}
