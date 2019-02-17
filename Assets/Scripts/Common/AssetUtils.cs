using UnityEngine;
using System.IO;

public static class AssetUtils
{
    public static Stream OpenRead(string path)
    {
        if (path.Contains("://"))
        {
            WWW reader = new WWW(path);
            while (!reader.isDone)
            {
            }
            return new MemoryStream(reader.bytes);
        }
        return File.OpenRead(path);
    }

    public static string ReadAllText(string path)
    {
        return new StreamReader(OpenRead(path)).ReadToEnd();
    }
}
