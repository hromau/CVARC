using System.IO;
using UnityEngine;

namespace Assets.Bundles
{
    public static class UriConstructor
    {
        public static string GetUriFileLocationPath(string fileName)
        {
            var assetBundleFolder = Path.GetFileName(Constants.PathToAssetBundles);
            var originPathToBundle = Path.Combine(Application.dataPath, assetBundleFolder);
            var originPath = Path.Combine(originPathToBundle, fileName);
            return "file:///" + originPath;
        }
    }
}
