using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Robot;

namespace HoMM.Sensors
{
    public abstract class BaseMapSensor<TUnitsCount> : Sensor<MapData<TUnitsCount>, IHommRobot>
    {
        public override MapData<TUnitsCount> Measure()
        {
            var players = Actor.World.Players
                .Where(p => p.Location.EuclideanDistance(Actor.Player.Location) <= Actor.ViewRadius);

            var objects = Actor.World.Round.Map
                .Where(x => x.Location.EuclideanDistance(Actor.Player.Location) <= Actor.ViewRadius)
                .Select(tile => BuildMapInfo(tile, players.FirstOrDefault(x => x.Location == tile.Location)));

            return new MapData<TUnitsCount>
            {
                Objects = objects.ToList(),
                Width = Actor.World.Round.Map.Width,
                Height = Actor.World.Round.Map.Height
            };
        }

        protected abstract Dictionary<UnitType, TUnitsCount> ConvertArmy(Dictionary<UnitType, int> internalRepresentation);

        private MapObjectData<TUnitsCount> BuildMapInfo(Tile tile, Player player)
        {
            var mapObjectData = new MapObjectData<TUnitsCount>
            {
                Location = tile.Location.ToLocationInfo(),
                Terrain = TerrainConverter[tile.Terrain]
            };

            if (player != null)
                mapObjectData.Hero = new Hero<TUnitsCount>(player.Name, ConvertArmy(player.Army));

            foreach (var obj in tile.Objects)
            {
                string owner = null;

                if (obj is CapturableObject)
                {
                    var capt = (CapturableObject) obj;
                    owner = capt.Owner?.Name;
                }

                if (obj is Wall)
                {
                    mapObjectData.Wall = new ClientClasses.Wall();
                }

                if (obj is Garrison)
                {
                    var army = ((Garrison) obj).Army;
                    mapObjectData.Garrison = new Garrison<TUnitsCount>(owner, ConvertArmy(army));
                }

                if (obj is NeutralArmy)
                {
                    var army = ((NeutralArmy) obj).Army;
                    mapObjectData.NeutralArmy = new NeutralArmy<TUnitsCount>(ConvertArmy(army));
                }

                if (obj is Mine)
                {
                    mapObjectData.Mine = new ClientClasses.Mine(((Mine) obj).Resource, owner);
                }

                if (obj is Dwelling)
                {
                    var dw = (Dwelling) obj;
                    mapObjectData.Dwelling = new ClientClasses.Dwelling(dw.Recruit.UnitType, dw.AvailableUnits, owner);
                }

                if (obj is ResourcePile)
                {
                    var rp = (ResourcePile) obj;
                    mapObjectData.ResourcePile = new ClientClasses.ResourcePile(rp.Resource, rp.Quantity);
                }
            }

            return mapObjectData;
        }

        private static readonly Dictionary<TileTerrain, Terrain> TerrainConverter = new Dictionary<TileTerrain, Terrain>()
        {
            {TileTerrain.Desert, Terrain.Desert },
            {TileTerrain.Grass, Terrain.Grass },
            {TileTerrain.Marsh, Terrain.Marsh },
            {TileTerrain.Road, Terrain.Road },
            {TileTerrain.Snow, Terrain.Snow }
        };
    }
}
