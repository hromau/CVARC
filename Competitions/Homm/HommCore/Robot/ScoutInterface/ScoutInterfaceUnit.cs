using HoMM.ClientClasses;
using HoMM.Engine;
using CVARC.V2;
using System;

namespace HoMM.Robot.ScoutInterface
{
    class ScoutInterfaceUnit : IUnit
    {
        private IHommRobot actor;

        public ScoutInterfaceUnit(IHommRobot actor)
        {
            this.actor = actor;
        }

        public UnitResponse ProcessCommand(object command)
        {
            var scoutOrder = Compatibility.Check<IScoutCommand>(this, command).ScoutOrder;
            if (scoutOrder == null) return UnitResponse.Denied();

            if (!scoutOrder.ScoutHero ^ scoutOrder.ScoutTile)
                throw new ArgumentException("Please scout either hero or tile at one time");
            if (scoutOrder.ScoutTile)
            {
                if (!scoutOrder.TileToScout.ToLocation().IsInside(actor.World.Round.Map.Size))
                    throw new ArgumentException("Scouted tile is out of map bounds");
                actor.Player.IsScoutingTile = true;
                actor.Player.TileBeingScouted = scoutOrder.TileToScout.ToLocation();
                return UnitResponse.Accepted(HommRules.Current.ScoutingDuration);
            }
            if (scoutOrder.ScoutHero)
            {
                actor.Player.IsScoutingHero = true;
                return UnitResponse.Accepted(HommRules.Current.ScoutingDuration);
            }
            throw new NotImplementedException("Not supposed to execute this");
        }
    }
}
