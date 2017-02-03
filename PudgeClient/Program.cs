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
            var client = new PudgeClientLevel3();
            client.Configurate(14000, new GameSettings(), new PudgeWorldState(123));

            client.Move(10);
            client.Hook();
        }
    }
}
