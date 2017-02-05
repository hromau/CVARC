using System;
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

        public void Work()
        {
            while (true)
            {
                var client = listener.AcceptTcpClient();
                var commandType = client.ReadJson<ServiceUnityCommand>();
                switch (commandType)
                {
                    case ServiceUnityCommand.Ping:
                        client.WriteJson("ping");
                        break;
                    case ServiceUnityCommand.GetCompetitionsList:
                        client.WriteJson(null);
                        break;
                    case ServiceUnityCommand.Shutdown:
                        client.WriteJson("ok");
                        client.Close();
                        Dispatcher.SetShutdown();
                        return;
                }
                client.Close();
            }
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
