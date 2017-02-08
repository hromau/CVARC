using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Infrastructure;
using CVARC.V2;

namespace ProxyCommon
{
    public class Proxy
    {
        private static readonly Random Rand = new Random(0);
        private readonly int proxyId;
        private readonly TcpClient from;
        private readonly TcpClient to;
        private bool cancelled;

        public static void CreateChainAndStart(TcpClient first, TcpClient second)
        {
            Task.Run(new Proxy(first, second).Work);
            Task.Run(new Proxy(second, first).Work);
        }

        public Proxy(TcpClient from, TcpClient to)
        {
            this.from = from;
            this.to = to;
            proxyId = Rand.Next(100);
        }

        public async Task Work()
        {
            try
            {
                while (!cancelled)
                {
                    if (!from.IsAlive())
                        break;
                    var bytes = await from.ReadAsync();
                    Debugger.Log("Translated " + System.Text.Encoding.UTF8.GetString(bytes));
                    if (!to.IsAlive())
                        break;
                    await to.WriteAsync(bytes);
                }
            }
            finally
            {
                Debugger.Log(proxyId + " closing");
                to.Close();
                from.Close();
                Debugger.Log(proxyId + " closed");
            }
        }

        public void Cancel() => cancelled = true;
    }
}
