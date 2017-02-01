using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityCommons
{
    public class PrefabNotFoundException : ApplicationException
    {
        static string messageTemplate = "Prefab `{0}` not found in bundle `{1}`!";

        public PrefabNotFoundException(string prefabName, string bundleName)
            : base(string.Format(messageTemplate, prefabName, bundleName)) { }
    }

    public static class PrefabLoader
    {
        private static AssetBundle _assetBundle;

        public static void SetAssetBundle(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
        }

        private static Object InstantiatePrefab(string name)
        {
            if (_assetBundle == null)
            {
                Debug.Log("====+++ not loaded module" + name);
                return null;
            }

            Debug.Log("====+++ using UnityEngine; name: " + name);
            return _assetBundle.LoadAsset(name);
        }

        public static T GetPrefab<T>(string name) where T : UnityEngine.Object
        {
            var prefab = InstantiatePrefab(name);
            if (prefab == null)
                throw new PrefabNotFoundException(name, _assetBundle.name);
            return (T)prefab;
        }
    }
}
