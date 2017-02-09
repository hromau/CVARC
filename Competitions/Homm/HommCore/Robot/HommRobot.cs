using CVARC.V2;
using HoMM.Engine;
using HoMM.Rules;
using HoMM.Sensors;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
using HoMM.World;
using System.Collections.Generic;
using System.Linq;
using System;
using Infrastructure;
using HoMM.ClientClasses;

namespace HoMM.Robot
{
    public class HommRobot : Robot<HommWorld, HommSensorData, HommCommand, HommRules>,
        IHommRobot
    {
        public override IEnumerable<IUnit> Units { get; }

        public Player Player { get; private set; }
        public IHommEngine HommEngine { get; }
        public Map Map => World.Round.Map;

        public HommRobot()
        {
            Units = new List<IUnit>
            {
                new HexMovUnit(this),
                new ArmyInterfaceUnit(this),
                new GarrisonBuilderUnit(this),
            };
        }

        public override void AdditionalInitialization()
        {
            base.AdditionalInitialization();

            if (World != null)
                Player = World.Players.Where(p => p.Name == ControllerId).Single();
        }

        public void Die()
        {
            Debugger.Log("Die!");

            World.CommonEngine.DeleteObject(ControllerId);
            var respawnTime = World.Clocks.CurrentTime + HommRules.Current.RespawnInterval;

            ControlTrigger.ScheduledTime = respawnTime + 0.001;

            World.Clocks.AddTrigger(new OneTimeTrigger(respawnTime, () =>
            {
                Player.Location = World.GetRespawnLocation(ControllerId, World.Round.Map);
                Player.DisiredLocation = Player.Location;

                World.HommEngine.CreateObject(ControllerId, MapObject.Hero, Player.Location.X, Player.Location.Y);
            }));
        }
    }
}
