using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace CVARC.V2
{
    public  interface INetworkController : IController
    {
        void InitializeClient(TcpClient client);
    }
}
