using CVARC.V2;
using HoMM.ClientClasses;
using HoMM.Robot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoMM.Sensors
{
    public class MapSensor : Sensor<MapData, HommRobot>
    {
        public override  MapData Measure()
        {
            var players = Actor.World.Players
                .Where(p => p.Location.EuclideanDistance(Actor.Player.Location) <= Actor.ViewRadius);

            var objects = Actor.World.Round.Map
                .Where(x => x.Location.EuclideanDistance(Actor.Player.Location) <= Actor.ViewRadius)
                .Select(tile => BuildMapInfo(tile, players.Where(x => x.Location == tile.Location).FirstOrDefault()));

            return new MapData()
            {
                Objects = objects.ToList(),
                Width = Actor.World.Round.Map.Width,
                Height = Actor.World.Round.Map.Height
            };
        }

        private static MapObjectData BuildMapInfo(Tile tile, Player player)
        {
            var mapInfo = new MapObjectData { Location = tile.Location.ToLocationInfo(), Terrain = terrain[tile.Terrain] };

            if (player != null)
                mapInfo.Hero = new Hero(player.Name, player.Army);

            foreach (var obj in tile.Objects)
            {
                string owner = null;

                if (obj is CapturableObject)
                {
                    var capt = (CapturableObject)obj;
                    owner = capt.Owner == null ? null : capt.Owner.Name;
                }

                if (obj is Wall)
                    mapInfo.Wall = new ClientClasses.Wall();

                if (obj is Garrison)
                    mapInfo.Garrison = new ClientClasses.Garrison(owner, ((Garrison)obj).Army);

                if (obj is NeutralArmy)
                    mapInfo.NeutralArmy = new ClientClasses.NeutralArmy(((NeutralArmy)obj).Army);

                if (obj is Mine)
                    mapInfo.Mine = new ClientClasses.Mine(((Mine)obj).Resource, owner);

                if (obj is Dwelling)
                {
                    var dw = (Dwelling)obj;
                    mapInfo.Dwelling = new ClientClasses.Dwelling(dw.Recruit.UnitType, dw.AvailableUnits);
                }

                if (obj is ResourcePile)
                {
                    var rp = (ResourcePile)obj;
                    mapInfo.ResourcePile = new ClientClasses.ResourcePile(rp.Resource, rp.Quantity);
                }
            }

            return mapInfo;
        }

        private static Dictionary<TileTerrain, Terrain> terrain = new Dictionary<TileTerrain, Terrain>()
        {
            {TileTerrain.Desert, Terrain.Desert },
            {TileTerrain.Grass, Terrain.Grass },
            {TileTerrain.Marsh, Terrain.Marsh },
            {TileTerrain.Road, Terrain.Road },
            {TileTerrain.Snow, Terrain.Snow }
        };
    }
}
