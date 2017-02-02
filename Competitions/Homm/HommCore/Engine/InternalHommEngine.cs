using System;
using System.Collections.Generic;
using UnityCommons;
using UnityEngine;

namespace HoMM.Engine
{
    class InternalHommEngine
    {
        //setspeed(enum), commonengine, coords, del color, коллайдеры, перейти на раунд, heroes

        private const float hexHeight = 1;
        private const int flagHeight = 1;

        private static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>
        {
            { "hex",  PrefabLoader.GetPrefab<GameObject>("hex") }
        };

        private Dictionary<MapObject, Func<GameObject>> objectFactory = new Dictionary<MapObject, Func<GameObject>>
        {
            { MapObject.Hexagon, () =>
                {
                    var obj = GameObject.Instantiate<GameObject>(prefabs["hex"]);
                    obj.transform.Rotate(90, 90, 0);
                    return obj;
                }
            },

            { MapObject.Mine, GetFactoryFor(PrimitiveType.Cube) },
            { MapObject.Flag, GetFactoryFor(PrimitiveType.Sphere) },
            { MapObject.Hero, GetFactoryFor(PrimitiveType.Capsule) },
            { MapObject.Dwelling, GetFactoryFor(PrimitiveType.Capsule) },
            { MapObject.NeutralArmy, GetFactoryFor(PrimitiveType.Cylinder) },
            { MapObject.ResourcesPile, GetFactoryFor(PrimitiveType.Sphere) },
        };

        private static Func<GameObject> GetFactoryFor(PrimitiveType primitive)
        {
            return () => GameObject.CreatePrimitive(primitive);
        }

        public GameObject CreateObject(string id, MapObject mapObject, int x = 0, int y = 0)
        {
            var obj = objectFactory.GetOrDefault(mapObject,
                () => GameObject.CreatePrimitive(PrimitiveType.Cube)).Invoke();

            GameObject.Destroy(obj.GetComponent(typeof(Collider)));
            SetPosition(obj, x, y);
            obj.name = id;
            return obj;
        }

        public void Move(GameObject obj, Direction direction, double duration)
        {
            /*var obj = GameObject.Find(id);
            obj.AddComponent<Rigidbody>();
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<Rigidbody>().isKinematic = true;
            var curpos = obj.transform.position;
            var targetCell = new Vector3();
            switch(direction) {
                case Directions.Up: targetCell = new Vector3(curpos.x, curpos.y, curpos.z + hexHeight); break;
            }
            var time = 1.0;
            var speed = new Vector3(targetCell.x - curpos.x, targetCell.y - curpos.y, targetCell.z - curpos.z);*/
            //speed.Normalize();
            //obj.GetComponent<Rigidbody>().velocity = speed;
            //commonEngine.SetAbsoluteSpeed(id, speed);
            //(())
            //while (time > 0) time -= Time.deltaTime;

            //obj.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            throw new NotImplementedException();
        }

        public void SetColor(GameObject obj, Color color)
        {
            foreach (var mr in obj.transform.GetComponentsInChildren<MeshRenderer>())
                mr.material.color = color;
        }

        public void SetScale(GameObject obj, float x, float y, float z)
        {
            obj.transform.localScale = new Vector3(x, y, z);
        }

        public void SetPosition(GameObject obj, int x, int y)
        {
            obj.transform.position = new Vector3(y, 0, x).ToUnityBasis(hexHeight);
        }

        public void SetFlag(GameObject obj, Color color)
        {
            var flagId = obj.name + " flag";

            var oldFlag = ObjectsCache.FindGameObject(flagId);
            if (oldFlag != null)
                GameObject.Destroy(oldFlag);

            var flag = CreateObject(flagId, MapObject.Flag);

            SetColor(flag, color);

            flag.transform.position = new Vector3(obj.transform.position.x, flagHeight, obj.transform.position.z);
            flag.transform.localScale = Vector3.one / 2;
        }

        public void SetCameraPosition(Vector3 coordinates)
        {
            ObjectsCache.FindGameObject("Camera").transform.position = coordinates;
        }

        public void SetCameraRotation(Quaternion rotation)
        {
            ObjectsCache.FindGameObject("Camera").transform.rotation = rotation;
        }
    }
}
