using System.Net.Sockets;
using System.Threading.Tasks;
using Infrastructure;
using CVARC.V2;

namespace ProxyCommon
{
    public class Proxy
    {
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
        }

        public async Task Work()
        {
            try
            {
                while (!cancelled && from.IsAlive() && to.IsAlive())
                {
                    var bytes = await from.ReadAsync();
                    Debugger.Log(System.Text.Encoding.ASCII.GetString(bytes));
                    await to.WriteAsync(bytes);
                }
            }
            finally
            {
                to.Close();
                from.Close();
            }
        }

        public void Cancel() => cancelled = true;
    }
}
