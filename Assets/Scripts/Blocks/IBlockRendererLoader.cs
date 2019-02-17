using UnityEngine;
using System.Collections;

public interface IBlockRendererLoader
{
    IBlockRenderer Load(string args);
}
