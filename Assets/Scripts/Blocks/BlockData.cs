using System;
using YamlSerializer;

[Serializable]
public class BlockData
{
    public string name;
    public bool isOpaque;
    public string renderer;
    public string bounds;
    public string texture;

    public static int GetContent(int value)
    {
        return value & 0b111111;
    }
}
