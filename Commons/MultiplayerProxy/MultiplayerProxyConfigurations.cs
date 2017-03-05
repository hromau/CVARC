using System;
using System.Collections.Generic;
using System.Net;
using Infrastructure;

namespace MultiplayerProxy
{
    public static class MultiplayerProxyConfigurations
    {
        public static readonly IPEndPoint ProxyEndPoint = new IPEndPoint(IPAddress.Any, 18700);
        public static readonly IPEndPoint UnityEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15000);
        public static readonly IPEndPoint ServiceEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14002);
        public static readonly string ApiKey = "huj";
        public static readonly string UriToCvarcTagList = "http://homm.ulearn.me/Teams/GetAllCvarcTags";
        public static readonly string UriToPutResult = "http://homm.ulearn.me/Games/Add";
        public static readonly TimeSpan CvarcTagListTimeToLive = TimeSpan.FromMinutes(5);
        
        public static readonly GameSettings DefaultGameSettings = new GameSettings
        {
            TimeLimit = 90,
            OperationalTimeLimit = 1000
        };
    }
}
