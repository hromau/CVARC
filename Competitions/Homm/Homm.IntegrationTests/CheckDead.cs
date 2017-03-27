using FluentAssertions;
using HoMM.ClientClasses;
using NUnit.Framework;

namespace Homm.IntegrationTests
{
    partial class Hero_should
    {
        [Test]
        public void GetFlagInSensorData_IfDead()
        {
            sensorData = client.Move(HoMM.Direction.RightDown);

            sensorData.IsDead.Should().BeTrue();

            sensorData = client.Wait(HommRules.Current.RespawnInterval);

            sensorData.IsDead.Should().BeFalse();
        }
    }
}
