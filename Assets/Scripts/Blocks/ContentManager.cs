using UnityEngine;
using System.IO;

public class ContentManager : MonoBehaviour
{
    public AssetBundle GetBundle(string name)
    {
        return AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
    }
}
