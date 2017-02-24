using System;
using System.Net;

namespace SingleplayerProxy
{
    public static class SingleplayerProxyConfigurations
    {
        public static bool DebugMode = false; // do not check and do not start unity
        public const bool UpdateEnabled = true;
        public const string PathToVersionFile = "version";
        public static readonly string PathToUnityDir = AppDomain.CurrentDomain.BaseDirectory;
        public const string UnityExePath = "ucvarc.exe";
        public const string UrlToGetVersion = "http://homm.ulearn.me/Service/GetVersion";
        public const string UrlToGetUpdate = "http://homm.ulearn.me/Service/GetUpdate";
        
        public static readonly IPEndPoint ProxyEndPoint = new IPEndPoint(IPAddress.Any, 18700);
        public static readonly IPEndPoint UnityEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15000);
        public static readonly IPEndPoint UnityServiceEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14002);
    }
}
