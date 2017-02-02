using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CVARC.V2;
using UnityCommons;
using UnityEngine;

namespace Assets.Bundles
{
    public class BundleLoader : MonoBehaviour
    {
        public void Start()
        {
            var bundleUrl = UriConstructor.GetUriFileLocationPath(Settings.Current.CurrentBundle);

            if (!Bundles.Cache.ContainsKey(bundleUrl))
                StartCoroutine(LoadBundle(bundleUrl));
        }

        private IEnumerator LoadBundle(string bundleUrl)
        {
            if (bundleUrl == null)
                throw new ArgumentException("Expected bundleUrl to be string, got null");

            WWW www = new WWW(bundleUrl);

            yield return www;

            if (www.error != null)
                throw new Exception("WWW error:" + www.error);

            if (!www.isDone)
                yield break;

            var loadedBundle = www.assetBundle;
            Bundles.Cache[bundleUrl] = loadedBundle;

            SendBundle(loadedBundle);
        }

        private void SendBundle(AssetBundle bundle)
        {
            // именно в таком порядке !
            PrefabLoader.SetAssetBundle(bundle);
            Dispatcher.FillLoader();

            Debugger.Log("WORLD: " + Application.dataPath);
        }
    }
}
