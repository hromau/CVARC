using System.Collections.Generic;
using System.Linq;
using HoMM.ClientClasses;

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
            var players = Actor.World.Players
                .Where(p => Actor.Player.IsScoutingHero ||
                       p.Location.EuclideanDistance(Actor.Player.Location) <= Actor.ViewRadius);
            if (Actor.Player.IsScoutingHero)
                Actor.Player.IsScoutingHero = false;

            var objects = Actor.World.Round.Map
                .Where(x => x.Location.EuclideanDistance(Actor.Player.Location) <= Actor.ViewRadius ||
                      (Actor.Player.IsScoutingTile &&
                        x.Location.EuclideanDistance(Actor.Player.TileBeingScouted) <= HommRules.Current.ScoutRadius))
                .Select(tile => BuildMapInfo(tile, players.FirstOrDefault(x => x.Location == tile.Location)));
            if (Actor.Player.IsScoutingTile)
            {
                Actor.Player.IsScoutingTile = false;
                Actor.Player.TileBeingScouted = null;
            }

            return new MapData<RoughQuantity>
            {
                Objects = objects.ToList(),
                Width = Actor.World.Round.Map.Width,
                Height = Actor.World.Round.Map.Height
            };
        }
    }
}
