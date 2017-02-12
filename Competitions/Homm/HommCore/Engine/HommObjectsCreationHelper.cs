using System;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;

namespace HoMM.Engine
{
    public class HommObjectsCreationHelper
    {
        private IHommEngine engine;

        Random random;

        private Dictionary<Resource, Color> resourceColor = new Dictionary<Resource, Color>
        {
            { Resource.Ebony, Color.magenta },
            { Resource.Glass, Color.green },
            { Resource.Gold, Color.yellow },
            { Resource.Iron, Color.cyan },
        };

        private Dictionary<UnitType, Color> unitColor = new Dictionary<UnitType, Color>
        {
            { UnitType.Cavalry, Color.magenta },
            { UnitType.Infantry, Color.cyan },
            { UnitType.Militia, Color.yellow },
            { UnitType.Ranged, Color.green },
        };

        public HommObjectsCreationHelper(Random random, IHommEngine engine)
        {
            this.random = random;
            this.engine = engine;
        }

        public void CreatePlayer(Player player, Color color)
        {
            engine.CreateObject(player.Name, MapObject.Hero, player.Location.X, player.Location.Y);
        }

        public void CreateHexagon(TileTerrain terrain, Location location)
        {
            var x = location.X;
            var y = location.Y;

            var hexId = string.Format("Tile {0} {1}", x, y);
            engine.CreateObject(hexId, GetHexagonTile(terrain), x, y);
        }

        private static MapObject GetHexagonTile(TileTerrain terrain)
        {
            if (terrain == TileTerrain.Grass) return MapObject.GrassTile;
            if (terrain == TileTerrain.Road) return MapObject.RoadTile;
            if (terrain == TileTerrain.Desert) return MapObject.DesertTile;
            if (terrain == TileTerrain.Marsh) return MapObject.MarshTile;
            if (terrain == TileTerrain.Snow) return MapObject.SnowTile;

            throw new ArgumentException();
        }

        public void CreateWall(Wall wall, TileTerrain terrain)
        {
            var wallType = GetRandomWallType(terrain);
            engine.CreateObject(wall.UnityId, wallType, wall.location.X, wall.location.Y);
        }

        private MapObject GetRandomWallType(TileTerrain terrain)
        {
            if (terrain == TileTerrain.Desert)
                return random.ChoiceWithProbability(0.3, MapObject.DeathForest, MapObject.Mountains);
            if (terrain == TileTerrain.Grass)
                return random.ChoiceWithProbability(0.75, MapObject.SummerForest, MapObject.Mountains);
            if (terrain == TileTerrain.Marsh)
                return random.ChoiceWithProbability(0.5, MapObject.SummerForest, MapObject.DeathForest);
            if (terrain == TileTerrain.Snow)
                return random.ChoiceWithProbability(0.75, MapObject.WinterForest, MapObject.Mountains);

            return MapObject.SummerForest;
        }

        public void CreateResourcePile(ResourcePile pile)
        {
            engine.CreateObject(pile.UnityId, MapObject.ResourcesPile, pile.location.X, pile.location.Y);

            var color = resourceColor[pile.Resource];
            engine.SetColor(pile.UnityId, color.r, color.g, color.b);
        }

        public void CreateDwelling(Dwelling dwelling, MapSize mapSize)
        {
            var dwellingType = GetDwellingType(dwelling.BuildingLocation, mapSize);
            engine.CreateObject(dwelling.UnityId, dwellingType, dwelling.BuildingLocation.X, dwelling.BuildingLocation.Y);

            CreateMarker(dwelling.UnityId, dwelling.BuildingLocation, dwelling.EntryLocation,
                unitColor[dwelling.Recruit.UnitType], MapObject.SwordMarker);
        }

        public void CreateMine(Mine mine)
        {
            engine.CreateObject(mine.UnityId, MapObject.Mine, mine.BuildingLocation.X, mine.BuildingLocation.Y);
            CreateMarker(mine.UnityId, mine.BuildingLocation, mine.EntryLocation, 
                resourceColor[mine.Resource], MapObject.PickaxeMarker);
        }

        private void CreateMarker(string id, Location building, Location entry, Color color, MapObject marker)
        {
            var hexmarkerId = id + " hexmarker";
            engine.CreateObject(hexmarkerId, MapObject.HexMarker, building.X, building.Y);
            engine.SetColor(hexmarkerId, color.r, color.g, color.b);

            var angle = building.GetDirectionTo(entry).ToUnityAngle() + Mathf.PI / 2;
            engine.SetRotation(hexmarkerId, angle);

            var markerId = id + " marker";
            engine.CreateObject(markerId, marker, entry.X, entry.Y);
            engine.SetColor(markerId, color.r, color.g, color.b);
        }

        private MapObject GetDwellingType(Location location, MapSize mapSize)
        {
            if (!location.IsInside(mapSize))
                return MapObject.Castle;

            return MapObject.Dwelling;
        }

        public void CreateArmy(ICombatable army, Location location)
        {
            var armyType = GetArmyType(army);
            engine.CreateObject(army.UnityId, armyType, location.X, location.Y);
            engine.SetRotation(army.UnityId, random.ChoiceWithProbability(0.5, Mathf.PI/3, 2*Mathf.PI/3));
        }

        private MapObject GetArmyType(ICombatable army)
        {
            var maxUnits = army.Army.Argmax(x => x.Value).Key;

            switch (maxUnits)
            {
                case UnitType.Cavalry: return MapObject.Cavalry;
                case UnitType.Infantry: return MapObject.Infantry;
                case UnitType.Militia: return MapObject.Militia;
                case UnitType.Ranged: return MapObject.Ranged;

                default: return MapObject.Militia;
            }
            
        }
    }
}
