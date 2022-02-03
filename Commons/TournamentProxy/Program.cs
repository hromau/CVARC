using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoMM.World;
using Infrastructure;
using Newtonsoft.Json.Linq;

namespace TournamentProxyNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = @"C:\itplanetfinal\data";
            var tasks = Json.Read<List<TournamentTask>>("tournament.json");
            var results = new List<TournamentGameResult>();
            if (File.Exists("results.json"))
                results = File.ReadLines("results.json").Select(Serializer.Deserialize<TournamentGameResult>).ToList();
            new TournamentProxy().Run(tasks, results);
        }

        static void MakeJsonExample()
        {
            Json.Write("..\\..\\tournament.json", new List<TournamentTask>()
            {
                new TournamentTask
                {
                    Participants = new List<TournamentParticipant>
                    {
                        new TournamentParticipant
                        {
                            Id = 1,
                            PathToExe =
                                "C:\\Projects\\CVARC\\Competitions\\Homm\\Homm.Client\\bin\\Debug\\Homm.Client.exe"
                        }
                    },
                    GameSettings = new GameSettings
                    {
                        LoadingData = new LoadingData
                        {
                            AssemblyName = "homm",
                            Level = "level1"
                        },
                        ActorSettings = new List<ActorSettings>
                        {
                            new ActorSettings
                            {
                                ControllerId = "Left",
                                IsBot = false,
                                PlayerSettings = new PlayerSettings
                                {
                                    CvarcTag = Guid.Empty
                                }
                            }
                        },
                        SpectacularView = true,
                        OperationalTimeLimit = 5,
                        TimeLimit = 90,
                        SpeedUp = false
                    },
                    WorldState = new JObject(new HommWorldState(123))
                }
            });
        }
    }
}