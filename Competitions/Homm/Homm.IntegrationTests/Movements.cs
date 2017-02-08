using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homm.IntegrationTests
{
    partial class Tests
    {
        [Test]
        public void DontMove()
        {
            Assert.AreEqual(0, sensorData.Location.X);
            Assert.AreEqual(0, sensorData.Location.Y);
        }

        [Test]
        public void MoveS()
        {
            sensorData = client.Move(HoMM.Direction.Down);
            Assert.AreEqual(0, sensorData.Location.X);
            Assert.AreEqual(1, sensorData.Location.Y);
        }
    }
}
