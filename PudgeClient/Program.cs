using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVARC.V2;
using Pudge;
using Pudge.World;
using Infrastructure;

namespace PudgeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Debugger.Logger = z => Console.WriteLine(z);
            Debugger.AlwaysOn = true;

            var client = new PudgeClientLevel1();
            client.Configurate("127.0.0.1", 18700, Guid.Empty);

            client.Move(10);
            client.Rotate(10);
        }
    }
}
