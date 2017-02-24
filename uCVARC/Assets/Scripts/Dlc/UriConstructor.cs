using System.IO;
using UnityEngine;

namespace Assets.Dlc
{
    public static class UriConstructor
    {
        /// <summary>
        /// Возвращает URI вида file:///Application.dataPath/filePath
        /// </summary>
        /// <param name="filePath">Путь к файлу относительно Application.dataPath</param>
        public static string GetUri(string filePath)
        {
            return "file:///" + Path.Combine(Path.GetFullPath("."), filePath);
        }

        public static string GetPathForDlcAssembly(string assemblyName)
        {
            return Path.Combine(Constants.PathToDlcAssemblies, assemblyName);
        }

        public static string GetUriForBundle(string bundleName)
        {
            return GetUri(Path.Combine(Constants.PathToAssetBundles, bundleName));
        }
    }
}
