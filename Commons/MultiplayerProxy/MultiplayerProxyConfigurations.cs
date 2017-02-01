using System;
using System.Collections.Generic;
using System.Net;

namespace MultiplayerProxy
{
    public static class MultiplayerProxyConfigurations
    {
        public static readonly IPEndPoint ProxyEndPoint = new IPEndPoint(IPAddress.Any, 18700);
        public static readonly IPEndPoint UnityEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15000);
        public static readonly string UriToCvarcTagList = "http://e1.ru/CvarcTag/Get";
        public static readonly string WebPassword = "SECRET";
        public static readonly TimeSpan CvarcTagListTimeToLive = TimeSpan.FromMinutes(5);
        public static readonly Dictionary<string, string[]> LevelToControllerIds = new Dictionary<string, string[]>
        {
            ["PudgeLevel1"] = new[] {"Left", "Right"}
        };
    }
}
