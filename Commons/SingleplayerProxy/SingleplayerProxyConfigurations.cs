using System.Net;

namespace SingleplayerProxy
{
    public static class SingleplayerProxyConfigurations
    {
        public const bool DebugMode = true; // do not check and do not start unity
        public const bool UpdateEnabled = false;
        public const string PathToVersionFile = "version";
        public const string UrlToGetVersion = "url";
        public const string UrlToGetUpdate = "url";
        public const string UrlToUnityDir = "unitydir";
        public const string UnityExePath = "Unity.exe";
        public const string UnityProcessName = "Unity.exe";
        public static readonly IPEndPoint ProxyEndPoint = new IPEndPoint(IPAddress.Any, 18700);
        public static readonly IPEndPoint UnityEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15000);
        public static readonly IPEndPoint UnityServiceEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14002);
    }
}
