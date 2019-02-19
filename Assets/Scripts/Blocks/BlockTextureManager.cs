using UnityEngine;
using System.Collections.Generic;

public class BlockTextureManager : MonoBehaviour
{
    public Texture2D MainTexture
    {
        get
        {
            return template;
        }
    }

    public Texture2D template;

    //Texture2D mainTex;
    Dictionary<string, Rect> texCoords = new Dictionary<string, Rect>();

    private void Start()
    {
        var blocks = GetComponent<ContentManager>().GetBundle("blocks");
        var texs = blocks.LoadAllAssets<Texture2D>();

        // template = new Texture2D(512, 512);
        template.filterMode = FilterMode.Point;
        template.mipMapBias = -0.5f;
        //uvs = mainTex.PackTextures(texs, 0, 512, true);
        PackTextures(template, texs, false);

        blocks.Unload(true);
    }
    private void PackTextures(Texture2D src, Texture2D[] texs, bool apply)
    {
        int mipmapCount = 6;
        for (int i = 0; i < mipmapCount; i++)
        {
            int texSize = 32 >> i;
            for (int k = 0; k < texs.Length; k++)
            {
                src.SetPixels((k & 15) * texSize, (k >> 4) * texSize, texSize, texSize, texs[k].GetPixels(i), i);
                texCoords[texs[k].name] = new Rect(0.0625f * (k & 15) + 0.0001f, 0.0625f * (k >> 4) + 0.0001f, 0.0623f, 0.0623f);
            }
        }
        texCoords["Error"] = new Rect(0.0625f * texs.Length, 0.0625f * texs.Length, 0.0625f, 0.0625f);
        src.Apply(true, apply);
    }

    public Rect FindBlockTexture(string name)
    {
        if (!texCoords.ContainsKey(name))
            throw new System.Exception($"block texture {name} not found");
        return texCoords[name];
    }
}
