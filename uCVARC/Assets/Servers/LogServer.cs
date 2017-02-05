using System;
using System.Net;
using System.Net.Sockets;
using Infrastructure;

namespace Assets.Servers
{
    public class LogServer : IDisposable
    {
        private TcpListener listener;

        public LogServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
        }

        public bool HasGame()
        {
            if (!listener.Pending())
                return false;

            var connection = listener.AcceptTcpClient();
            Dispatcher.LogModel = connection.ReadJson<LogModel>();
            connection.Close();
            return true;
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
