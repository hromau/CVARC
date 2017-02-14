using HoMM;
using HoMM.ClientClasses;
using NUnit.Framework;
using System;
using HoMM.Engine;

namespace Homm.IntegrationTests
{
    public partial class Tests
    {
        HommClient<HommSensorData> client;
        HommSensorData sensorData;
        private const int PilePrice = 1000;
        private const int MaxResourceCount = 3000;
        [SetUp]
        public void Init()
        {
            client = new HommClient<HommSensorData>();
            sensorData = client.Configurate("127.0.0.1", 18700, Guid.Empty, operationalTimeLimit: 5, debugMap:true);
        }

        private void MoveHero(Direction direction, int stepsCount)
        {
            for (var i = 0; i < stepsCount; i++)
            {
                sensorData = client.Move(direction);
            }
        }

        private void MoveHero(params Direction [] sequenceOfDirections)
        {
            foreach (var direction in sequenceOfDirections)
            {
                sensorData = client.Move(direction);
            }
        }

        [TearDown]
        public void Dispose()
        {
            client.Exit();
        }
    }
}
