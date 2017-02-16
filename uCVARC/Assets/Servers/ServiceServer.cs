using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Infrastructure;

namespace Assets.Servers
{
    public class ServiceServer : IDisposable
    {
        private readonly TcpListener listener;

        public ServiceServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        }

        private Dictionary<LoadingData, string[]> GetControllersIdInfo()
        {
            return Dispatcher.Loader.Levels
                .SelectMany(x => x.Value.Keys, (x, y) => new LoadingData {AssemblyName = x.Key, Level = y})
                .ToDictionary(l => l, l => Dispatcher.Loader.GetCompetitions(l).Logic.Actors.Keys.ToArray());
        }

        public void Work()
        {
            while (true)
            {
                var client = listener.AcceptTcpClient();
                try
                {
                    var commandType = client.ReadJson<ServiceUnityCommand>();
                    Debugger.Log("Accepted service command " + commandType.ToString());
                    switch (commandType)
                    {
                        case ServiceUnityCommand.Ping:
                            client.WriteJson("ping");
                            break;
                        case ServiceUnityCommand.GetCompetitionsList:
                            client.WriteJson(GetControllersIdInfo());
                            break;
                        case ServiceUnityCommand.Shutdown:
                            client.WriteJson("ok");
                            client.Close();
                            Dispatcher.SetShutdown();
                            return;
                    }
                }
                catch (Exception) { }
                finally { client.Close(); }
            }
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
