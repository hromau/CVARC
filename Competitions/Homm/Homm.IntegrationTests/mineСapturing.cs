using System.Linq;
using FluentAssertions;
using HoMM;
using HoMM.Rules;
using NUnit.Framework;

namespace Homm.IntegrationTests
{
    partial class Hero_should
    {
        [Test]
        public void AllMinesHaventOwner_WhenGameStarts()
        {
            sensorData.Map.Objects.Select(mapObj => mapObj.Mine)
                                  .Where(mine => mine != null)
                                  .All(mine => mine.Owner == null).Should().BeTrue();
        }

        [Test]
        public void DontEarnIncome_WhenDontOwnAnyMines()
        {
            MoveToMinesLocation();
            sensorData.MyTreasury.Values.Sum().Should().Be(0);
        }

        [Test]
        public void MinesHasOwner_WhenHeroCaptured()
        {
            CaptureAllMines();
            sensorData.Map.Objects.Select(mapObj => mapObj.Mine)
                                  .Where(mine => mine != null)
                                  .All(mine => mine.Owner != null).Should().BeTrue();
        }

        [Test]
        public void GainResources_WhenOwnMines()
        {
            CaptureAllMines();
            MoveHero(Direction.Down);
            MoveHero(Direction.RightDown, 10);
            sensorData.MyTreasury.Values.All(val => val == HommRules.Current.MineCaptureScores)
                                        .Should().BeTrue();

        }

        public void CaptureAllMines()
        {
            MoveToMinesLocation();
            MoveHero(Direction.RightUp);
            for (var i = 0; i < 3; i++)
            {
                MoveHero(Direction.Down);
            }
        }

        private void MoveToMinesLocation()
        {
            MoveHero(Direction.Down);
            for (var i = 0; i < 2; i++)
            {
                MoveHero(Direction.RightDown);
                MoveHero(Direction.RightUp);
            }
            MoveHero(Direction.RightDown);
        }   
    }
}
