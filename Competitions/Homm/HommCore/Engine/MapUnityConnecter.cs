using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace HoMM.Engine
{
    public static class MapUnityConnecter
    {
        private static HommEngine engine;
        private static Dictionary<MapObject, int> objects = new Dictionary<MapObject, int>() {
            {MapObject.Mine, 0 },
            {MapObject.Dwelling, 0 },
            {MapObject.ResourcesPile, 0 },
            {MapObject.NeutralArmy, 0 }
        };

        public static void Connect(Round round, HommEngine e) {
            var map = round.Map;
            engine = e;
            engine.SetCamera(map.Width, map.Height);
            engine.CreatePlayers(round.Players.Select(p => p.Name).ToArray());
            foreach (var location in Location.Square(map.Size)) { 
                    engine.CreateHexagon(GetHexagonType(map[location].tileTerrain), location.X, location.Y);
                    CreateTileObject(map[location].tileObject);
            }
        }

        private static TerrainType GetHexagonType(TileTerrain terrain) {
            if (terrain == TileTerrain.Grass) return TerrainType.Grass;
            if (terrain == TileTerrain.Road) return TerrainType.Road;
            if (terrain == TileTerrain.Arid) return TerrainType.Arid;
            if (terrain == TileTerrain.Desert) return TerrainType.Desert;
            if (terrain == TileTerrain.Marsh) return TerrainType.Marsh;
            if (terrain == TileTerrain.Snow) return TerrainType.Snow;
            return TerrainType.Undefined;
        }

        private static void CreateTileObject(TileObject tileObject) {
            if (tileObject != null) {
                var x = tileObject.location.X;
                var y = tileObject.location.Y;
                if (tileObject is Mine) {
                    tileObject.unityID = $"Mine {objects[MapObject.Mine]++}";
                    engine.CreateObject(tileObject.unityID, MapObject.Mine, x, y);
                }
                if (tileObject is Dwelling) {
                    tileObject.unityID = $"Dwelling {objects[MapObject.Dwelling]++}";
                    engine.CreateObject(tileObject.unityID, MapObject.Dwelling, x, y);
                }
                if (tileObject is ResourcePile) {
                    tileObject.unityID = $"Resources pile {objects[MapObject.ResourcesPile]++}";
                    engine.CreateObject(tileObject.unityID, MapObject.ResourcesPile, x, y);
                }
                if (tileObject is NeutralArmy) {
                    tileObject.unityID = $"Neutral army {objects[MapObject.NeutralArmy]++}";
                    engine.CreateObject(tileObject.unityID, MapObject.NeutralArmy, x, y);
                }

                engine.SetSize(tileObject.unityID, 0.5f, 0.5f, 0.5f);

                if (tileObject is CapturableObject) {
                   var owner = (tileObject as CapturableObject).Owner;
                   engine.SetFlag(tileObject.unityID, owner == null? "" : owner.Name);
                }

                ConnectTileObject(tileObject);
            }
        }


        private static void ConnectTileObject(TileObject tileObject) {
            if (tileObject == null) return;

            tileObject.Remove += DeleteHandler;

            if (tileObject is INotifyPropertyChanged) {
                ((INotifyPropertyChanged)tileObject).PropertyChanged += UpdateHandler;
            }
        }

        private static void UpdateHandler(object sender, PropertyChangedEventArgs e) {
            TileObject obj;
            try {
                obj = (TileObject)sender;
            }
            catch (InvalidCastException) {
                Console.WriteLine("UpdateHandler: wrong sender"); // log
                return;
            }

            if (e.PropertyName == "Owner") {
                var owner = ((CapturableObject)obj).Owner;
                engine.SetFlag(obj.unityID, owner.Name);
            }
        }

        private static void DeleteHandler(TileObject obj) {
            engine.DeleteObject(obj.unityID);
        }
    }
}
