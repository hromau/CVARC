using Assets.Bundles;
using UnityEditor;

public class BuildBundler
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        BuildPipeline.BuildAssetBundles(BundleConstants.PathToAssetBundles, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}