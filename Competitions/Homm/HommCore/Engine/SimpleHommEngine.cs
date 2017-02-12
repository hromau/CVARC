using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HoMM.Engine
{
    class SimpleHommEngine : BaseHommEngine
    {
        private static Dictionary<MapObject, Func<GameObject>> objectFactory = new Dictionary<MapObject, Func<GameObject>>
        {
            { MapObject.Hero,  As(PrimitiveType.Capsule, Color.white) },
            { MapObject.Infantry,  As(PrimitiveType.Cylinder, Color.cyan) },
            { MapObject.Ranged,  As(PrimitiveType.Cylinder, Color.green) },
            { MapObject.Cavalry, As(PrimitiveType.Cylinder, Color.magenta) },
            { MapObject.Militia, As(PrimitiveType.Cylinder, Color.yellow) },

            { MapObject.RoadTile, As(HommPrefabs.ByMapObject[MapObject.RoadTile]) },
            { MapObject.SnowTile, As(HommPrefabs.ByMapObject[MapObject.SnowTile]) },
            { MapObject.GrassTile, As(HommPrefabs.ByMapObject[MapObject.GrassTile]) },
            { MapObject.DesertTile, As(HommPrefabs.ByMapObject[MapObject.DesertTile]) },
            { MapObject.MarshTile, As(HommPrefabs.ByMapObject[MapObject.MarshTile]) },

            { MapObject.SummerForest, As(PrimitiveType.Cube, Color.grey) },
            { MapObject.DeathForest, As(PrimitiveType.Cube, Color.grey) },
            { MapObject.WinterForest, As(PrimitiveType.Cube, Color.grey) },
            { MapObject.Mountains, As(PrimitiveType.Cube, Color.grey) },

            { MapObject.Mine, As(PrimitiveType.Cube, Color.white) },
            { MapObject.Dwelling, As(PrimitiveType.Cube, Color.white) },
            { MapObject.ResourcesPile, As(PrimitiveType.Sphere, Color.white) },
            { MapObject.Castle, As(PrimitiveType.Cube, Color.grey) },

            { MapObject.Flag, As(HommPrefabs.ByMapObject[MapObject.Flag]) },
            { MapObject.HexMarker, As(HommPrefabs.ByMapObject[MapObject.HexMarker]) },
            { MapObject.CircleMarker, As(HommPrefabs.ByMapObject[MapObject.CircleMarker]) },
            { MapObject.SwordMarker, As(HommPrefabs.ByMapObject[MapObject.SwordMarker]) },
            { MapObject.PickaxeMarker, As(HommPrefabs.ByMapObject[MapObject.PickaxeMarker]) },
        };

        private static Func<GameObject> As(PrimitiveType primitive, Color color)
        {
            return () =>
            {
                var obj = GameObject.CreatePrimitive(primitive);
                obj.GetComponent<MeshRenderer>().material.color = color;
                obj.transform.localScale *= 0.5f;
                return obj;
            };
        }

        private static Func<GameObject> As(GameObject prefab)
        {
            return () => GameObject.Instantiate<GameObject>(prefab);
        }

        protected override GameObject Instantiate(MapObject mapObject)
        {
            return objectFactory.GetOrDefault(mapObject, As(PrimitiveType.Cube, Color.white)).Invoke();
        }
    }
}
