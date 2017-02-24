using CVARC.V2;
using System;
using System.Collections.Generic;
using Infrastructure;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityCommons
{
    public class AssetNotFoundException : ApplicationException
    {
        static string messageTemplate = "Asset `{0}` is not found in bundle `{1}`!";

        public AssetNotFoundException(string assetName, string bundleName)
            : base(string.Format(messageTemplate, assetName, bundleName)) { }
    }

    public class BundleNotFoundException : ApplicationException
    {
        static string messageTemplate = "Bundle `{0}` is not loaded!";

        public BundleNotFoundException(string bundleName)
            : base(string.Format(messageTemplate, bundleName)) { }
    }

    public static class AssetLoader
    {
        private static Dictionary<string, AssetBundle> _assetBundles;

        public static void SetAssetBundles(Dictionary<string, AssetBundle> bundles)
        {
            _assetBundles = bundles;
        }

        private static Object LoadAsset(string bundle, string name, Type type)
        {
            if (_assetBundles == null)
            {
                Debugger.Log($"Bundles are not loaded: `{nameof(_assetBundles)}` is null");
                return null;
            }

            if (!_assetBundles.ContainsKey(bundle))
                throw new BundleNotFoundException(bundle);

            return _assetBundles[bundle].LoadAsset(name, type);
        }

        public static T LoadAsset<T>(string bundle, string name) where T : Object
        {
            var asset = LoadAsset(bundle, name, typeof(T)) as T;
            if (asset == null)
                throw new AssetNotFoundException(name, bundle);
            return asset;
        }
    }
}
