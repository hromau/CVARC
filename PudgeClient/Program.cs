using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVARC.V2;
using Pudge;
using Pudge.World;
using Infrastructure;

namespace PudgeClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Debugger.Logger = z => Console.WriteLine(z);
            Debugger.AlwaysOn = true;

            var client = new PudgeClient();
            client.Configurate("127.0.0.1", 18700, Guid.Empty);

            client.Move(100);
            client.Rotate(100);
            client.Blink(0, 0);
            client.CreateWard();
            client.Wait(100);
            client.Hook();
        }
    }
}
