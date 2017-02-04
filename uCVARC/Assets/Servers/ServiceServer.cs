using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Infrastructure;

namespace Assets.Servers
{
    public class ServiceServer : IDisposable
    {
        private readonly TcpListener listener;
        private readonly int version;

        public ServiceServer(int port, int version)
        {
            this.version = version;
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
                    case ServiceUnityCommand.GetVersion:
                        client.WriteJson(version);
                        break;
                    case ServiceUnityCommand.Shutdown:
                        client.WriteJson("ok");
                        Dispatcher.SetShutdown();
                        break;
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
