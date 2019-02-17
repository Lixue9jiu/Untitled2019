using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class BlockManager : MonoBehaviour
{
    public BlockData[] Blocks { get; private set; }
    internal Dictionary<string, object>[] mainData;

    Dictionary<string, int> blockIds = new Dictionary<string, int>();

    public bool IsOpaque(int index)
    {
        return Blocks[index].isOpaque;
    }

    public int FindBlock(string name)
    {
        if (!blockIds.ContainsKey(name))
            throw new System.Exception($"block {name} not found");
        return blockIds[name];
    }

    private void Start()
    {
        List<BlockDataGroup> groups = YamlUtils.LoadBlockGroups(AssetUtils.ReadAllText(Path.Combine(Application.streamingAssetsPath, "Blocks.yml")));
        AssignBlockIds(groups);
    }

    private void AssignBlockIds(List<BlockDataGroup> groups)
    {
        List<BlockData> blocks = new List<BlockData>();
        blocks.Add(new BlockData { name = "Air", isOpaque = false });
        blockIds["game:air"] = 0;
        int index = 1;
        foreach (BlockDataGroup group in groups)
        {
            string groupName = group.name.ToLower();
            for (int i = 0; i < group.blocks.Length; i++)
            {
                blockIds[groupName + ':' + group.blocks[i].name.ToLower().Replace(' ', '_')] = index;
                blocks.Add(group.blocks[i]);
                index++;
            }
        }
        Blocks = blocks.ToArray();
    }
}
