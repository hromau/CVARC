﻿using CVARC.V2;
using HoMM.Engine;
using HoMM.Sensors;
using HoMM.Robot.ArmyInterface;
using HoMM.Robot.HexagonalMovement;
using HoMM.World;
using System.Collections.Generic;
using System.Linq;
using System;
using Infrastructure;
using HoMM.ClientClasses;
using HoMM.Robot.ScoutInterface;

namespace HoMM.Robot
{
    public interface IHommRobot : IActor
    {
        new HommWorld World { get; }
        Player Player { get; }
        Map Map { get; }
        IHommEngine HommEngine { get; }
        double ViewRadius { get; }
        bool IsDead { get; }
        void Die();
    }

    public class HommRobot<TSensorData> : Robot<HommWorld, TSensorData, HommCommand, HommRules>, IHommRobot
        where TSensorData : new()
    {
        public override IEnumerable<IUnit> Units { get; }

        public Player Player { get; private set; }
        public IHommEngine HommEngine { get; }
        public Map Map => World.Round.Map;
        public double ViewRadius { get; }

        public bool IsDead { get; private set; }

        public HommRobot(double visibilityRadius)
        {
            ViewRadius = visibilityRadius;

            Units = new List<IUnit>
            {
                new HexMovUnit(this),
                new ArmyInterfaceUnit(this),
                new GarrisonBuilderUnit(this),
                new ScoutInterfaceUnit(this),
            };
        }

        public override void AdditionalInitialization()
        {
            base.AdditionalInitialization();

            if (World != null)
                Player = World.Players.Single(p => p.Name == ControllerId);

            Debugger.Log($"Initialize robot. ObjectId: {ObjectId}, ControllerId: {ControllerId}");
        }

        public void Die()
        {
            Debugger.Log("Die!");

            IsDead = true;

            World.CommonEngine.DeleteObject(ControllerId);

            var respawnTime = World.Clocks.CurrentTime + HommRules.Current.RespawnInterval - 0.001;

            ControlTrigger.ScheduledTime = World.Clocks.CurrentTime;

            World.Clocks.AddTrigger(new OneTimeTrigger(respawnTime, () =>
            {
                Player.Location = World.GetRespawnLocation(ControllerId, World.Round.Map);
                Player.DesiredLocation = Player.Location;

                World.HommEngine.CreateObject(ControllerId, MapObject.Hero, Player.Location.X, Player.Location.Y);

                IsDead = false;
            }));
        }
    }
}
