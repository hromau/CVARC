using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityCommons
{
    public static class PrefabLoader
    {
        private static AssetBundle _assetBundle;

        public static void SetAssetBundle(AssetBundle assetBundle)
        {
            _assetBundle = assetBundle;
        }

        public static Object InstantiatePrefab(string name)
        {
            if (_assetBundle == null)
            {
                Debug.Log("====+++ not loaded module" + name);
                return null;
            }

            Debug.Log("====+++ using UnityEngine; name: " + name);
            return _assetBundle.LoadAsset(name);
        }
    }
}
