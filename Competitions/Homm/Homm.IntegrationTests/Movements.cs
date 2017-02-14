using NUnit.Framework;
using HoMM;
using HoMM.ClientClasses;

namespace Homm.IntegrationTests
{
    partial class Tests
    {
        [Test]
        public void DontMove()
        {
            Assert.True(IsOnTheStartLocation(sensorData.Location));
        }

        private bool IsOnTheStartLocation(LocationInfo curentLocation)
        {
            return curentLocation.X == 0 && curentLocation.Y == 0;
        }

        [Test]
        public void DontMove_WhenOutside()
        {
            sensorData = client.Move(Direction.Up);
            Assert.True(IsOnTheStartLocation(sensorData.Location));
        }

        [Test]
        public void DontMove_WhenWall()
        {
            sensorData = client.Move(Direction.Down);
            MoveHero(Direction.RightDown, 5);
            Assert.AreEqual(3, sensorData.Location.X);
            Assert.AreEqual(2, sensorData.Location.Y);
        }

        [Test]
        public void MoveUp()
        {
            sensorData = client.Move(Direction.Down);
            sensorData = client.Move(Direction.Up);
            Assert.True(IsOnTheStartLocation(sensorData.Location));
        }

        [Test]
        public void MoveDown()
        {
            sensorData = client.Move(Direction.Down);
            Assert.AreEqual(0, sensorData.Location.X);
            Assert.AreEqual(1, sensorData.Location.Y);
        }

        [Test]
        public void MoveRightDown()
        {
            sensorData = client.Move(Direction.Down);
            sensorData = client.Move(Direction.RightDown);
            Assert.AreEqual(1, sensorData.Location.X);
            Assert.AreEqual(1, sensorData.Location.Y);
        }

        [Test]
        public void MoveRightUp()
        {
            MoveHero(Direction.Down, 2);
            sensorData = client.Move(Direction.RightUp);
            Assert.AreEqual(1, sensorData.Location.X);
            Assert.AreEqual(1, sensorData.Location.Y);
        }

        [Test]
        public void MoveLeftUp()
        {
            MoveHero(Direction.Down, Direction.RightDown, Direction.LeftUp);
            Assert.AreEqual(0, sensorData.Location.X);
            Assert.AreEqual(1, sensorData.Location.Y);
        }

        [Test]
        public void MoveLeftDown()
        {
            MoveHero(Direction.Down, Direction.RightDown, Direction.LeftDown);
            Assert.AreEqual(0, sensorData.Location.X);
            Assert.AreEqual(2, sensorData.Location.Y);
        }

        [Test]
        public void Move_WhenResourcePileOnTheWay()
        {
            MoveHero(Direction.Down, 2);
            Assert.AreEqual(0, sensorData.Location.X);
            Assert.AreEqual(2, sensorData.Location.Y);
        }
    }
}
