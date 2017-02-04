using CVARC.V2;
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
            { "hex",  PrefabLoader.GetPrefab<GameObject>("homm", "hex") }
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

        public void Freeze(GameObject obj)
        {
            var rigidbody = obj.GetComponent<Rigidbody>();
            if (rigidbody != null) rigidbody.velocity = Vector3.zero;
        }

        public void Move(GameObject obj, Direction direction, float duration)
        {
            Debugger.Log("Move called");

            var rigidbody = obj.GetComponent<Rigidbody>();

            if (rigidbody == null)
            {
                rigidbody = obj.AddComponent<Rigidbody>();
                rigidbody.useGravity = false;
            }

            rigidbody.velocity = GetVelocity(direction).normalized / duration;

            Debugger.Log(rigidbody.velocity);
        }

        private Vector3 GetVelocity(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return new Vector3(1, 0, 0);

                case Direction.Up:
                    return new Vector3(-1, 0, 0);

                case Direction.RightDown:
                    return new Vector3(0.5f, 0, 0.5f * Mathf.Sqrt(3));

                case Direction.LeftDown:
                    return new Vector3(0.5f, 0, -0.5f * Mathf.Sqrt(3));

                case Direction.RightUp:
                    return new Vector3(-0.5f, 0, 0.5f * Mathf.Sqrt(3));

                case Direction.LeftUp:
                    return new Vector3(-0.5f, 0, -0.5f * Mathf.Sqrt(3));

                default:
                    throw new ArgumentException($"Invalid direction: {direction}");
            }
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
            obj.transform.position = FromHexagonal(x, y);
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

        private Vector3 FromHexagonal(int x, int y)
        {
            var newX = y * hexHeight + (x % 2 * hexHeight / 2);
            var newZ = (3 * hexHeight * x) / (2 * Mathf.Sqrt(3));
            return new Vector3(newX, 0, newZ);
        }
    }
}
