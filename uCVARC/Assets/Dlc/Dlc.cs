using System.Collections.Generic;
using UnityEngine;

namespace Assets.Dlc
{
    public static class Dlc
    {
        public static Dictionary<string, AssetBundle> BundleCache = new Dictionary<string, AssetBundle>();
        public static HashSet<string> AssemblyCache = new HashSet<string>();
    }
}
