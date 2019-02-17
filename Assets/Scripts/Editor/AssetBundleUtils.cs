using System.IO;
using UnityEngine;
using UnityEditor;

public static class AssetBundleUtils
{
    [MenuItem("Assets/Build Asset Bundle")]
    static void BuildAssetBundle()
    {
        string assetBundleDir = "Assets/AssetBundle";
        if (!Directory.Exists(assetBundleDir))
        {
            Directory.CreateDirectory(assetBundleDir);
        }
#if UNITY_ANDROID
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.Android);
#endif
    }
}
