using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ContentManager : MonoBehaviour
{
    Dictionary<string, AssetBundle> m_bundles = new Dictionary<string, AssetBundle>();

    public AssetBundle GetBundle(string name)
    {
        if (!m_bundles.ContainsKey(name))
            m_bundles[name] = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, name));
        return m_bundles[name];
    }
}
