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
        private string _bundleUrl;
        private AssetBundle _bundle;

        public void Start()
        {
            _bundleUrl = UriConstructor.GetUriFileLocationPath(Settings.CurrentBundle);
            StartCoroutine(Work());
        }

        private IEnumerator Work()
        {
            if (_bundleUrl == null)
            {
                Debug.Log("Trouble with bundleUrl");
                yield break;
            }
            WWW www = new WWW(_bundleUrl);
            yield return www;
            if (www.error != null)
                throw new Exception("Was error:" + www.error);
            if (!www.isDone)
                yield break;
            _bundle = www.assetBundle;
            SendBundle();
        }

        public void SendBundle()
        {
            // именно в таком порядке !
            PrefabLoader.SetAssetBundle(_bundle);
            Dispatcher.FillLoader();

            Debugger.Log(DebuggerMessageType.Initialization, "WORLD: " + Application.dataPath);
        }
    }
}
