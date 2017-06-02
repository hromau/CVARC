using HoMM.World;
using Infrastructure;
using MultiplayerProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TournamentProxyNamespace
{
    class Program
    {



        static void Main(string[] args)
        {
            Environment.CurrentDirectory = @"D:\itplanetfinal\data";
            var tasks = Json.Read<List<TournamentTask>>("tournament.json");
            var results = new List<TournamentGameResult>();
            if (File.Exists("results.json"))
                results = File.ReadLines("results.json").Select(JsonConvert.DeserializeObject<TournamentGameResult>).ToList();
            new TournamentProxy().Run(tasks,results);
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
                         Id=1,
                         PathToExe="C:\\Repos\\CVARC\\Competitions\\Homm\\Homm.Client\\bin\\Debug\\Homm.Client.exe"
                    }
                },
                GameSettings = new GameSettings
                {
                    LoadingData = new LoadingData
                    {
                        AssemblyName = "homm",
                        Level = "level1"
                    },
                    ActorSettings=new List<ActorSettings>
                    {
                        new ActorSettings
                        {
                             ControllerId="Left",
                              IsBot=false,
                               PlayerSettings=new PlayerSettings
                               {
                                    CvarcTag=Guid.Empty
                               }
                        }
                    },
                    SpectacularView=true,
                     OperationalTimeLimit=5,
                      TimeLimit = 90,
                       SpeedUp=false
                },
                WorldState = JObject.FromObject(new HommWorldState(123))
            } });
        }
    }
}
