using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Robot.ScoutInterface;
using System;

namespace HoMM
{
    public class Scout
    {
        private WorldClocks clocks;
        private double timeOfLastUsage = Double.NegativeInfinity;

        public bool IsScoutingTile { get; private set; }
        public bool IsScoutingHero { get; private set; }
        public Location TileBeingScouted { get; private set; }

        public Scout(WorldClocks clocks)
        {
            this.clocks = clocks;
        }

        public double Execute(ScoutOrder order)
        {
            if (!order.ScoutHero ^ order.ScoutTile)
                throw new ArgumentException("Please scout either hero or tile at one time");

            IsScoutingTile = order.ScoutTile;
            IsScoutingHero = order.ScoutHero;
            TileBeingScouted = order.TileToScout?.ToLocation();

            timeOfLastUsage = clocks.CurrentTime;

            return HommRules.Current.ScoutingDuration;
        }

        public void Reset()
        {
            IsScoutingHero = false;
            IsScoutingTile = false;
            TileBeingScouted = null;
        }

        public bool IsAvailable()
        {
            return clocks.CurrentTime > timeOfLastUsage + HommRules.Current.ScoutingCooldown;
        }
    }
}
