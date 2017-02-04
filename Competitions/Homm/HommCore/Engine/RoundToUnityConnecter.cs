using CVARC.V2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace HoMM.Engine
{
    public class RoundToUnityConnecter
    {
        private IHommEngine hommEngine;
        private ICommonEngine commonEngine;
        private HommEngineHighLevelApi api;

        private List<Color> availablePlayerColors = new List<Color> { Color.red, Color.blue };
        private Dictionary<string, Color> playerColorMapping = new Dictionary<string, Color>();

        public RoundToUnityConnecter(IHommEngine hommEngine, ICommonEngine commonEngine)
        {
            this.hommEngine = hommEngine;
            this.commonEngine = commonEngine;
            api = new HommEngineHighLevelApi(hommEngine);
        }

        private static Dictionary<MapObject, int> counter = new Dictionary<MapObject, int>
        {
            { MapObject.Mine, 0 },
            { MapObject.Dwelling, 0 },
            { MapObject.ResourcesPile, 0 },
            { MapObject.NeutralArmy, 0 },
            { MapObject.Wall, 0 }
        };

        public void Connect(Round round)
        {
            var map = round.Map;

            api.SetCameraToCenter(map.Width, map.Height);

            for (var i = 0; i < round.Players.Length; ++i)
            {
                var player = round.Players[i];
                var color = availablePlayerColors[i];
                playerColorMapping[player.Name] = color;

                api.CreatePlayer(player.Name, color, player.Location);
            }

            foreach (var location in Location.Square(map.Size))
            {
                api.CreateHexagon(GetTerrainType(map[location].Terrain), location);

                foreach (var tileObject in map[location].Objects)
                    CreateTileObject(tileObject);
            }
        }

        private static TerrainType GetTerrainType(TileTerrain terrain)
        {
            if (terrain == TileTerrain.Grass) return TerrainType.Grass;
            if (terrain == TileTerrain.Road) return TerrainType.Road;
            if (terrain == TileTerrain.Arid) return TerrainType.Arid;
            if (terrain == TileTerrain.Desert) return TerrainType.Desert;
            if (terrain == TileTerrain.Marsh) return TerrainType.Marsh;
            if (terrain == TileTerrain.Snow) return TerrainType.Snow;
            return TerrainType.Undefined;
        }

        private void CreateTileObject(TileObject tileObject)
        {
            if (tileObject == null) return;

            var x = tileObject.location.X;
            var y = tileObject.location.Y;

            if (tileObject is Mine)
            {
                tileObject.unityID = $"Mine {counter[MapObject.Mine]++}";
                hommEngine.CreateObject(tileObject.unityID, MapObject.Mine, x, y);
            }
            if (tileObject is Dwelling)
            {
                tileObject.unityID = $"Dwelling {counter[MapObject.Dwelling]++}";
                hommEngine.CreateObject(tileObject.unityID, MapObject.Dwelling, x, y);
            }
            if (tileObject is ResourcePile)
            {
                return;
                tileObject.unityID = $"Resources pile {counter[MapObject.ResourcesPile]++}";
                hommEngine.CreateObject(tileObject.unityID, MapObject.ResourcesPile, x, y);
            }
            if (tileObject is NeutralArmy)
            {
                tileObject.unityID = $"Neutral army {counter[MapObject.NeutralArmy]++}";
                hommEngine.CreateObject(tileObject.unityID, MapObject.NeutralArmy, x, y);
            }
            if (tileObject is Wall)
            {
                tileObject.unityID = $"Wall {counter[MapObject.Wall]++}";
                hommEngine.CreateObject(tileObject.unityID, MapObject.Wall, x, y);
            }

            if (tileObject.unityID == null)
            {
                throw new ArgumentException($"Got TileObject of unknown type: {tileObject.GetType().Name}");
            }

            hommEngine.SetScale(tileObject.unityID, 0.5f, 0.5f, 0.5f);

            if (tileObject is CapturableObject)
            {
                var owner = (tileObject as CapturableObject).Owner;
                var color = playerColorMapping.GetOrDefault(owner?.Name, Color.grey);
                hommEngine.SetFlag(tileObject.unityID, color.r, color.g, color.b);
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
                var color = playerColorMapping.GetOrDefault(owner?.Name, Color.grey);
                hommEngine.SetFlag(obj.unityID, color.r, color.g, color.b);
            }
        }

        private void DeleteHandler(TileObject obj)
        {
            commonEngine.DeleteObject(obj.unityID);
        }
    }
}
