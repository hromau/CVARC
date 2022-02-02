
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Infrastructure;
using MultiplayerProxy;
using Newtonsoft.Json.Linq;
using ProxyCommon;

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

        public void Run(List<TournamentTask> list, List<TournamentGameResult> results)
        {
            
            int counter = -1;
            foreach (var task in list)
            {
                counter++;
                if (results.Any(z => z.Task.Id == task.Id)) continue;

                Console.WriteLine($"Task {counter} of {list.Count}");

                var serviceConnection = new TcpClient();
                serviceConnection.Connect(MultiplayerProxyConfigurations.UnityEndPoint);
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

                File.AppendAllText("results.json", Serializer.Serialize(toWrite) + "\n");

                Thread.Sleep(1000);
            }
        }
        
        Process RunAndAccept(TournamentParticipant p)
        {
            var process = new Process();
            process.StartInfo.FileName = p.PathToExe;
            process.StartInfo.Arguments = $"127.0.0.1 {tournamentProxyPort}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.StartInfo.ErrorDialog = false;
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
