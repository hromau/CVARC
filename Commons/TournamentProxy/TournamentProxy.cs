using Infrastructure;
using MultiplayerProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProxyCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TournamentProxyNamespace
{
    class TournamentProxy : IDisposable
    {
        TcpListener listener;
        const int tournamentProxyPort = 31312;

        public TournamentProxy()
        {
            listener = new TcpListener(tournamentProxyPort);
            listener.Start();
        }

        public void Run(List<TournamentTask> list)
        {
            var serviceConnection = new TcpClient();
            serviceConnection.Connect(MultiplayerProxyConfigurations.UnityEndPoint);

            foreach (var task in list)
            {
                serviceConnection.WriteJson(task.GameSettings);
                serviceConnection.WriteJson(task.WorldState);

                var clients = task.Participants
                    .Select(RunAndAccept)
                    .ToList();

                var result = serviceConnection.ReadJson<GameResult>();
                foreach (var c in clients)
                    if (!c.HasExited)
                        c.Kill();
                    
                var toWrite = new TournamentGameResult
                {
                    Task = task,
                    Result = result
                };

                File.AppendAllText("results.json", JsonConvert.SerializeObject(toWrite, Formatting.None) + "\n");
            }
        }

        Process RunAndAccept(TournamentParticipant p)
        {
            var process = new Process();
            process.StartInfo.FileName = p.PathToExe;
            process.StartInfo.Arguments = $"127.0.0.1 {tournamentProxyPort}";
            process.Start();
            var watch = new Stopwatch();
            watch.Start();
            while(watch.ElapsedMilliseconds<1000)
            {
                if (listener.Pending())
                {
                    var client = listener.AcceptTcpClient();
                    client.ReadJson<JObject>();
                    client.ReadJson<JObject>();
                    var unityClient = new TcpClient();
                    unityClient.Connect(MultiplayerProxyConfigurations.UnityEndPoint);
                    Proxy.CreateChainAndStart(client, unityClient);
                    return process;
                }
            }
            throw new Exception();
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
