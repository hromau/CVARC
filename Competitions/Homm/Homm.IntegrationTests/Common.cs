using HoMM;
using HoMM.ClientClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homm.IntegrationTests
{
    public partial class Tests
    {
        HommClient<HommSensorData> client;
        HommSensorData sensorData;

        [SetUp]
        public void Init()
        {
            client = new HommClient<HommSensorData>();
            sensorData = client.Configurate("127.0.0.1", 18700, Guid.Empty, operationalTimeLimit: 5);
        }

        [TearDown]
        public void Dispose()
        {
            client.Exit();
        }
    }
}
