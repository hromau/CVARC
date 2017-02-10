using NUnit.Framework;
using Pudge;
using Pudge.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pudge.IntegrationTests
{
    [TestFixture]
    public partial class Tests
    {
        PudgeClient client;

        PudgeSensorsData sensorData;

        [SetUp]
        public void Setup()
        {
            client = new PudgeClient() { LevelName = "Debug" };
            sensorData = client.Configurate("127.0.0.1", 18700, Guid.Empty, operationalTimeLimit: 5);
        }
    }
}
