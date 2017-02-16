using System;
using System.Collections;
using System.IO;
using UnityCommons;
using UnityEngine;
using System.Reflection;
using System.Linq;
using Infrastructure;

namespace Assets.Dlc
{
    public class DlcLoader : MonoBehaviour
    {
        public void Start()
        {
            Debugger.Log("DlcLoader started");
            LoadBundles();
            LoadAssemblies();
            PrefabLoader.SetAssetBundles(Dlc.BundleCache);
        }

        private void LoadBundles()
        {
            foreach (var bundleName in Settings.Current.DlcBundles)
            {
                Debugger.Log("Loading bundle " + bundleName);
                if (!Dlc.BundleCache.ContainsKey(bundleName.ToLower()))
                {
                    var bundleUrl = UriConstructor.GetUriForBundle(bundleName);
                    StartCoroutine(LoadBundle(bundleName, bundleUrl));
                    Debugger.Log("Loaded bundle " + bundleName);
                }
                else
                {
                    Debugger.Log("Already loaded bundle " + bundleName);
                }
                
            }
        }

        private void LoadAssemblies()
        {
            foreach (var assemblyName in Settings.Current.DlcAssemblies)
            {
                Debugger.Log("Loading assembly " + assemblyName);
                if (!Dlc.AssemblyCache.Contains(assemblyName.ToLower()))
                {
                    var assemblyUrl = UriConstructor.GetPathForDlcAssembly(assemblyName);
                    LoadAssembly(assemblyUrl);
                    Dlc.AssemblyCache.Add(assemblyName);
                    Debugger.Log("Loaded assembly " + assemblyName);
                }
                else
                {
                    Debugger.Log("Already loaded assembly " + assemblyName);
                }
            }
        }

        private void LoadAssembly(string assemblyName)
        {
            var assembly = Assembly.Load(File.ReadAllBytes(assemblyName));

            var entryPointType = assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract || !t.IsInterface)
                .Where(t => typeof(IDlcEntryPoint).IsAssignableFrom(t))
                .Single();

            var entryPoint = (IDlcEntryPoint)Activator.CreateInstance(entryPointType);

            Dispatcher.FillLoader(entryPoint);
        }

        private IEnumerator LoadBundle(string bundleName, string bundleUrl)
        {
            bundleName = bundleName.ToLower();
            bundleUrl = bundleUrl.ToLower();

            if (bundleUrl == null)
                throw new ArgumentException("Expected bundleUrl to be string, got null");

            WWW www = new WWW(bundleUrl);
            yield return www;

            if (www.error != null)
                throw new Exception("WWW error:" + www.error);

            while (!www.isDone)
                yield break;

            if (www.assetBundle == null) throw new InvalidDataException();

            Dlc.BundleCache[bundleName] = www.assetBundle;
        }

        //private void SendBundles()
        //{
        //    PrefabLoader.SetAssetBundles(Dlc.BundleCache);
        //    Dispatcher.FillLoader();
        //}
    }
}
