using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityCommons;
using UnityEngine;

namespace HoMM.Engine
{
    class FancyHommEngine : BaseHommEngine
    {
        public override void SetUpScene(int width, int height)
        {
            base.SetUpScene(width, height);

            var desk = PrefabLoader.GetPrefab<GameObject>("homm", "desktop");
            GameObject.Instantiate(desk);
        }

        protected override GameObject Instantiate(MapObject mapObject)
        {
            var prefab = HommPrefabs.ByMapObject.GetOrDefault(mapObject, null);

            return prefab == null
                ? GameObject.CreatePrimitive(PrimitiveType.Cube)
                : GameObject.Instantiate<GameObject>(prefab);
        }
    }
}
