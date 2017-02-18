using CVARC.V2;
using Infrastructure;
using System;
using System.Collections.Generic;
using UnityCommons;
using UnityEngine;

namespace HoMM.Engine
{
    abstract class BaseHommEngine
    {
        private const float hexWidth = 1;

        private Dictionary<string, Color> heroColors = new Dictionary<string, Color>();

        private Dictionary<Animation, string> animationName = new Dictionary<Animation, string>
        {
            { Animation.Attack, "attack" },
            { Animation.Gallop, "gallop" },
            { Animation.Idle, "idle" },
        };

        private Queue<Color> availableColors = new Queue<Color>();

        public BaseHommEngine()
        {
            availableColors.Enqueue(Color.red);
            availableColors.Enqueue(Color.blue);
        }

        public virtual void SetUpScene(int width, int height)
        {
            var camera = ObjectsCache.FindGameObject("Camera");

            camera.transform.position = new Vector3(width / 2 + 12, 10, width / 2 - 1);
            camera.transform.rotation = Quaternion.Euler(45, -90, 0);

            camera.GetComponent<Camera>().fieldOfView = 45;

            var light = ObjectsCache.FindGameObject("Main Light").GetComponent<Light>();

            light.intensity = 1.3f;
            light.shadowStrength = 0.8f;
        }

        public void InitColor(GameObject hero)
        {
            if (!heroColors.ContainsKey(hero.name))
            {
                var color = availableColors.Dequeue();
                heroColors[hero.name] = color;
            }

            SetColor(hero, heroColors[hero.name]);
        }

        public GameObject CreateObject(string id, MapObject mapObject, int x = 0, int y = 0)
        {
            var obj = Instantiate(mapObject);

            GameObject.Destroy(obj.GetComponent<Collider>());
            SetPosition(obj, x, y);
            obj.name = id;

            switch (mapObject)
            {
                case MapObject.Hero:
                    InitColor(obj);
                    break;
                case MapObject.Militia:
                case MapObject.Ranged:
                    obj.transform.localScale *= 0.9f;
                    break;
                case MapObject.Cavalry:
                    obj.transform.localScale *= 1.35f;
                    obj.transform.Translate(0, 0.06f, 0);
                    break;
            }

            return obj;
        }

        protected abstract GameObject Instantiate(MapObject mapObject);

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
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            }

            var movementDirectionVector = GetVelocity(direction).normalized;
            rigidbody.velocity = movementDirectionVector / duration;

            Debugger.Log(rigidbody.velocity);
        }

        private Vector3 GetVelocity(Direction direction)
        {
            var angle = direction.ToUnityAngle();
            return new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
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

        public void SetRotation(GameObject obj, float angleRadians)
        {
            var angleDegrees = angleRadians * 180 / Mathf.PI;

            Debugger.Log($"SetRotation: {angleDegrees} degrees / {angleRadians} radians");

            obj.transform.rotation = Quaternion.AngleAxis((float)angleDegrees, Vector3.up);
        }

        public void SetFlag(GameObject obj, string ownerId)
        {
            var flagId = obj.name + " flag";

            var oldFlag = ObjectsCache.FindGameObject(flagId);
            if (oldFlag != null)
                GameObject.Destroy(oldFlag);

            var flag = CreateObject(flagId, MapObject.Flag);

            SetColor(flag, heroColors.GetOrDefault(ownerId, Color.white));

            flag.transform.position = obj.transform.position + Vector3.up * 0.3f;
            flag.transform.parent = obj.transform;
        }

        private Vector3 FromHexagonal(int x, int y)
        {
            var newX = y * hexWidth + (Math.Abs(x) % 2 * hexWidth / 2);
            var newZ = (3 * hexWidth * x) / (2 * Mathf.Sqrt(3));
            return new Vector3(newX, 0, newZ);
        }

        public void SetAnimation(GameObject obj, Animation animation)
        {
            var animationStr = animationName[animation];

            foreach (var animator in obj.GetComponentsInChildren<Animator>())
            {
                animator.Play(animationStr);
            }
        }
    }
}
