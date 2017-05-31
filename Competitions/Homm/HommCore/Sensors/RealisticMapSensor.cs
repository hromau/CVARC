using System.Collections.Generic;
using HoMM.ClientClasses;
using HoMM.Robot;

namespace HoMM.Sensors
{
    public class RealisticMapSensor : BaseMapSensor<RoughQuantity>
    {
        protected override Dictionary<UnitType, RoughQuantity> ConvertArmy(Dictionary<UnitType, int> internalRepresentation)
        {
            return new RealisticArmy(internalRepresentation);
        }

        public override MapData<RoughQuantity> Measure()
        {
            var mapData = base.Measure();
            Actor.Player.Scout.Reset();
            return mapData;
        }

        protected override bool PlayerFilter(Player player, IHommRobot robot)
        {
            return robot.Player.Scout.IsScoutingHero || robot.Player.Scout.IsScoutingTile || base.PlayerFilter(player, robot);
        }

        protected override bool TileFilter(Tile tile, IHommRobot actor, Player playerOnTile)
        {
            var scout = actor.Player.Scout;
            var scoutRadius = HommRules.Current.ScoutRadius;

            var tileIsObservableByTileScout = 
                scout.IsScoutingTile && tile.Location.EuclideanDistance(scout.TileBeingScouted) <= scoutRadius;

            var tileIsObservableByHeroScout =
                scout.IsScoutingHero && playerOnTile != null;

            var tileIsObservableByScout = tileIsObservableByTileScout || tileIsObservableByHeroScout;

            return tileIsObservableByScout || base.TileFilter(tile, actor, playerOnTile);
        }
    }
}
