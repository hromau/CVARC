using System;
using System.Linq;
using CVARC.V2;
using Pudge.Units.HookUnit;
using Pudge.Units.WADUnit;
using Pudge.World;

namespace Pudge.Tests
{
    class HookTests : DefaultPudgeTest
    {
        [CvarcTestMethod]
        public void Hook_ShouldAddHookEventWhenThrown()
        {
            Robot.Hook();

            AssertTrue(s => s.Events
                .Where(e => e.Event == PudgeEvent.HookThrown)
                .FirstOrDefault() != null);
        }

        [CvarcTestMethod]
        public void Hook_DirectHitShouldBeLethalToPudge()
        {
            Robot
                .Rotate(-45)
                .GameMove(100)
                .Rotate(90)
                .GameMove(100)
                .Rotate(-45)
                .GameMove(180)
                .Wait()
                .Hook()
                .Wait();
            
            AssertEqual(s => s.Map.Heroes
                .Where(d => d.Type == Sensors.Map.HeroType.Pudge)
                .Count(), 0);
        }

        [CvarcTestMethod]
        public void Hook_DirectHitShouldBeLethalToSlardar()
        {
            Robot
                .Rotate(-45)
                .GameMove(80)
                .Rotate(90)
                .GameMove(80)
                .Rotate(90)
                .GameMove(20)
                .Rotate(-90)
                .GameMove(80)
                .Wait()
                .Hook()
                .Wait();

            AssertEqual(s => s.Map.Heroes
                .Where(d => d.Type == Sensors.Map.HeroType.Pudge)
                .Count(), 0);
        }

        [CvarcTestMethod]
        public void Hook_ShouldNotWalkWhileHookingWithoutWaitCall()
        {
            Robot.Rotate(-45).GameMove(1).Hook();

            AssertEqual(s => s.SelfLocation.X, -122, 0.5);
            AssertEqual(s => s.SelfLocation.Y, -122, 0.5);
        }

        [CvarcTestMethod]
        public void Hook_ShouldNotRotateWhileHookingWithoutWaitCall()
        {
            Robot.Rotate(1).Hook();

            AssertEqual(s => s.SelfLocation.Angle, 50, 0.5);
        }

        [CvarcTestMethod]
        public void Hook_ShouldNotCollideWithTrees()
        {
            Robot
                .Rotate(45)
                .Wait()
                .Hook();

            AssertTrue(s => s.Events
                .Where(e => e.Event == PudgeEvent.HookThrown)
                .FirstOrDefault() != null);
        }
    }
}
