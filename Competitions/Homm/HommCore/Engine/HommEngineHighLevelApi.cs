using System;
using UnityEngine;

namespace HoMM.Engine
{
    class HommEngineHighLevelApi
    {
        private IHommEngine engine;

        public HommEngineHighLevelApi(IHommEngine engine)
        {
            this.engine = engine;
        }

        public void CreatePlayer(string playerId, Color color, Location location)
        {
            engine.CreateObject(playerId, MapObject.Hero, location.X, location.Y);
            engine.SetColor(playerId, color.r, color.g, color.b);
            engine.SetScale(playerId, 0.5f, 0.5f, 0.5f);
        }

        public void CreateHexagon(TerrainType terrain, Location location)
        {
            var x = location.X;
            var y = location.Y;

            var hexId = string.Format("Tile {0} {1}", x, y);
            engine.CreateObject(hexId, MapObject.Hexagon, x, y);

            var color = GetHexagonColor(terrain);
            engine.SetColor(hexId, color.r, color.g, color.b);
        }

        private static Color GetHexagonColor(TerrainType terrain)
        {
            switch (terrain)
            {
                case TerrainType.Grass: return Color.green;
                case TerrainType.Road: return Color.grey;
                case TerrainType.Arid: return new Color32(0xDA, 0xA5, 0x20, 1);
                case TerrainType.Desert: return Color.yellow;
                case TerrainType.Marsh: return new Color32(0x00, 0x64, 0x00, 1);
                case TerrainType.Snow: return Color.white;

                default: return Color.black;
            }
        }

        public void SetCameraToCenter(float mapWidth, float mapHeight)
        {
            engine.SetCameraPosition(mapWidth / 2, 17, mapHeight / 2);
            engine.SetCameraRotation(90, -90, 0);
        }
    }

    public static class HexagonalCoordsConverter
    {
        public static Vector3 ToUnityBasis(this Vector3 hexagonalCoordinates, float hexSize = 1)
        {
            var hc = hexagonalCoordinates;

            var newX = hc.x * hexSize + (hc.z % 2 * hexSize / 2);
            var newZ = (3 * hexSize * hc.z) / (2 * (float)Math.Sqrt(3));

            return new Vector3(newX, 0, newZ);
        }
    }
}
