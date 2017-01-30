using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using Pudge.Units.PudgeUnit;
using Pudge.Units.WADUnit;

namespace Pudge.Tests
{
    public class RuneTriggerTests : DefaultPudgeTest
    {
        public readonly LocatorItem CurrentLocation = new LocatorItem { Angle = 45, X = -130, Y = -130 };

        //[CvarcTestMethod]
        //public void Runes_PickBountyRune()
        //{
        //    Robot.GameMove(18);
        //    AssertEqual(data => data.SelfScores, 10);
        //}
    }
}
