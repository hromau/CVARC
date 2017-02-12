using CVARC.V2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using Infrastructure;
using UnityEngine;

using Random = System.Random;

namespace HoMM.Engine
{
    class RoundToUnityConnecter
    {
        private IHommEngine hommEngine;
        private ICommonEngine commonEngine;
        private HommObjectsCreationHelper objectsCreationHelper;

        private List<Color> availablePlayerColors = new List<Color> { Color.red, Color.blue };
        private Dictionary<string, Color> playerColorMapping = new Dictionary<string, Color>();

        public RoundToUnityConnecter(IHommEngine hommEngine, ICommonEngine commonEngine, 
            HommObjectsCreationHelper objectsCreationHelper)
        {
            this.hommEngine = hommEngine;
            this.commonEngine = commonEngine;
            this.objectsCreationHelper = objectsCreationHelper;
        }

        private static Dictionary<MapObject, int> counter = new Dictionary<MapObject, int>
        {
            { MapObject.Mine, 0 },
            { MapObject.Dwelling, 0 },
            { MapObject.ResourcesPile, 0 },
            { MapObject.Infantry, 0 },
            { MapObject.Wall, 0 }
        };

        public void Connect(Round round)
        {
            var map = round.Map;

            hommEngine.SetUpScene(map.Width, map.Height);

            for (var i = 0; i < round.Players.Length; ++i)
            {
                var player = round.Players[i];
                var color = availablePlayerColors[i];
                playerColorMapping[player.Name] = color;

                Debugger.Log("Creating player...");
                objectsCreationHelper.CreatePlayer(player, color);
            }

            var buildings = new HashSet<Location>(map
                .SelectMany(x => x.Objects)
                .Where(x => x is IBuilding)
                .Cast<IBuilding>()
                .Select(x => x.BuildingLocation));

            foreach (var location in Location.Square(map.Size))
            {
                var tile = map[location];

                objectsCreationHelper.CreateHexagon(tile.Terrain, location);

                foreach (var tileObject in tile.Objects)
                    if (!(tileObject is Wall && buildings.Contains(location)))
                        CreateTileObject(tileObject, tile.Terrain, map.Size);
            }
        }

        private void CreateTileObject(TileObject tileObject, TileTerrain terrain, MapSize mapSize)
        {
            if (tileObject == null) return;

            var x = tileObject.location.X;
            var y = tileObject.location.Y;

            if (tileObject is IBuilding)
            {
                var buildingLocation = ((IBuilding)tileObject).BuildingLocation;
                x = buildingLocation.X;
                y = buildingLocation.Y;
            }

            if (tileObject is Mine)
            {
                tileObject.UnityId = $"Mine {counter[MapObject.Mine]++}";
                objectsCreationHelper.CreateMine((Mine)tileObject);
            }
            if (tileObject is Dwelling)
            {
                tileObject.UnityId = $"Dwelling {counter[MapObject.Dwelling]++}";
                objectsCreationHelper.CreateDwelling((Dwelling)tileObject, mapSize);
            }
            if (tileObject is ResourcePile)
            {
                tileObject.UnityId = $"Resources pile {counter[MapObject.ResourcesPile]++}";
                objectsCreationHelper.CreateResourcePile((ResourcePile)tileObject);
            }
            if (tileObject is NeutralArmy)
            {
                tileObject.UnityId = $"Neutral army {counter[MapObject.Infantry]++}";
                objectsCreationHelper.CreateArmy((NeutralArmy)tileObject, tileObject.location);
            }
            if (tileObject is Wall)
            {
                tileObject.UnityId = $"Wall {counter[MapObject.Wall]++}";
                objectsCreationHelper.CreateWall((Wall)tileObject, terrain);
            }

            if (tileObject.UnityId == null)
            {
                throw new ArgumentException($"Got TileObject of unknown type: {tileObject.GetType().Name}");
            }

            if (tileObject is CapturableObject)
            {
                var owner = (tileObject as CapturableObject).Owner;
                hommEngine.SetFlag(tileObject.UnityId, owner?.Name ?? string.Empty);
            }

            ConnectTileObject(tileObject);
        }

        private void ConnectTileObject(TileObject tileObject)
        {
            if (tileObject == null) return;

            tileObject.Remove += DeleteHandler;

            if (tileObject is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)tileObject).PropertyChanged += UpdateHandler;
            }
        }

        private void UpdateHandler(object sender, PropertyChangedEventArgs e)
        {
            TileObject obj;

            try
            {
                obj = (TileObject)sender;
            }
            catch (InvalidCastException)
            {
                Debugger.Log($"Expected `{nameof(sender)}` to be TileObject, but got {sender.GetType().Name}. Ignore it.");
                return;
            }

            if (e.PropertyName == "Owner")
            {
                var owner = ((CapturableObject)obj).Owner;
                hommEngine.SetFlag(obj.UnityId, owner?.Name ?? string.Empty);
            }
        }

        private void DeleteHandler(TileObject obj)
        {
            commonEngine.DeleteObject(obj.UnityId);
        }
    }
}
