using CVARC.V2;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityCommons
{
    public class PrefabNotFoundException : ApplicationException
    {
        static string messageTemplate = "Prefab `{0}` is not found in bundle `{1}`!";

        public PrefabNotFoundException(string prefabName, string bundleName)
            : base(string.Format(messageTemplate, prefabName, bundleName)) { }
    }

    public class BundleNotFoundException : ApplicationException
    {
        static string messageTemplate = "Bundle `{0}` is not loaded!";

        public BundleNotFoundException(string bundleName)
            : base(string.Format(messageTemplate, bundleName)) { }
    }

    public static class PrefabLoader
    {
        private static Dictionary<string, AssetBundle> _assetBundles;

        public static void SetAssetBundles(Dictionary<string, AssetBundle> bundles)
        {
            _assetBundles = bundles;
        }

        private static Object InstantiatePrefab(string bundle, string name)
        {
            if (_assetBundles == null)
            {
                Debugger.Log($"Bundles are not loaded: `{nameof(_assetBundles)}` is null");
                return null;
            }

            if (!_assetBundles.ContainsKey(bundle))
                throw new BundleNotFoundException(bundle);

            return _assetBundles[bundle].LoadAsset(name);
        }

        public static T GetPrefab<T>(string bundle, string name) where T : UnityEngine.Object
        {
            var prefab = InstantiatePrefab(bundle, name);
            if (prefab == null)
                throw new PrefabNotFoundException(name, bundle);
            return (T)prefab;
        }
    }
}
