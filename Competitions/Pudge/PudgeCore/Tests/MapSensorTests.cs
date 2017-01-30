using CVARC.V2;
using Pudge.Units.PudgeUnit;
using Pudge.Units.WADUnit;

namespace Pudge.Tests
{
    public class MapSensorTests : DefaultPudgeTest
    {
        //[CvarcTestMethod]
        //public void MapSensor_SimpleMapSensorTest()
        //{
        //    Robot.Wait();
        //    AssertEqual(s => s.Map.Runes.Count, 2);
        //}

        //[CvarcTestMethod]
        //public void MapSensor_ShouldSeeOnlyOneRune()
        //{
        //    Robot.Rotate(WADCommandType.RotateCounterClockwise, 9).GameMove(5);
        //    AssertEqual(s => s.Map.Runes.Count, 1);
        //}

        //[CvarcTestMethod]
        //public void MapSensor_ShouldSeeOnlyOneRuneAfterTaking()
        //{
        //    Robot.Rotate(WADCommandType.RotateCounterClockwise, 9)
        //        .GameMove(6)
        //        .Rotate(WADCommandType.RotateClockwise, 9 * 4)
        //        .GameMove(6);
        //    AssertEqual(s => s.Map.Runes.Count, 1);
        //}

        [CvarcTestMethod]
        public void ShouldSeeHeroesOnMap()
        {
            Robot.Wait();
            AssertEqual(s => s.Map.Heroes.Count, 1);
        }
    }
}