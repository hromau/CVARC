using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using CVARC.V2;
using UnityCommons;
using UnityEngine;

namespace Assets.Bundles
{
    public class BundlesLoader : MonoBehaviour
    {
        public void Start()
        {
            foreach (var bundleName in Settings.Current.BundlesToLoad)
            {
                if (!Bundles.Cache.ContainsKey(bundleName))
                {
                    var bundleUrl = UriConstructor.GetUriFileLocationPath(bundleName);
                    Bundles.Cache[bundleName] = LoadBundle(bundleName, bundleUrl);
                }
            }

            SendBundles();
        }

        private AssetBundle LoadBundle(string bundleName, string bundleUrl)
        {
            if (bundleUrl == null)
                throw new ArgumentException("Expected bundleUrl to be string, got null");

            WWW www = new WWW(bundleUrl);

            if (www.error != null)
                throw new Exception("WWW error:" + www.error);

            return www.assetBundle;
        }

        private void SendBundles()
        {
            PrefabLoader.SetAssetBundles(Bundles.Cache);
            Dispatcher.FillLoader();
        }
    }
}
