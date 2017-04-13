using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityCommons;
using UnityEngine;

namespace HoMM.Engine
{
    static class HommPrefabs
    {
        public static Dictionary<MapObject, GameObject> ByMapObject = new Dictionary<MapObject, GameObject>
        {
            { MapObject.Hero,  LoadPrefab("hero") },
            { MapObject.Infantry,  LoadPrefab("infantry") },
            { MapObject.Ranged,  LoadPrefab("ranged") },
            { MapObject.Cavalry, LoadPrefab("cavalry") },
            { MapObject.Militia, LoadPrefab("militia") },

            { MapObject.RoadTile, LoadPrefab("roadTile") },
            { MapObject.SnowTile, LoadPrefab("snowTile") },
            { MapObject.GrassTile, LoadPrefab("grassTile") },
            { MapObject.DesertTile, LoadPrefab("desertTile") },
            { MapObject.MarshTile, LoadPrefab("marshTile") },

            { MapObject.SummerForest, LoadPrefab("summerForest") },
            { MapObject.DeathForest, LoadPrefab("deathForest") },
            { MapObject.WinterForest, LoadPrefab("winterForest") },
            { MapObject.Mountains, LoadPrefab("mountains") },

            { MapObject.Mine, LoadPrefab("mine") },
            { MapObject.Dwelling, LoadPrefab("dwelling") },
            { MapObject.ResourcesPile, LoadPrefab("resources") },
            { MapObject.Castle, LoadPrefab("castle") },

            { MapObject.Flag, LoadPrefab("flag") },
            { MapObject.HexMarker, LoadPrefab("hexMarker") },
            { MapObject.CircleMarker, LoadPrefab("circleMarker") },
            { MapObject.SwordMarker, LoadPrefab("swordMarker") },
            { MapObject.PickaxeMarker, LoadPrefab("pickaxeMarker") },
        };
        
        private static GameObject LoadPrefab(string name)
        {
            return AssetLoader.LoadAsset<GameObject>("homm", name);
        }
    }
}
