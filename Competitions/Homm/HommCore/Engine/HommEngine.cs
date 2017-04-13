using System;
using CVARC.V2;
using UnityCommons;
using UnityEngine;

namespace HoMM.Engine
{
    public class HommEngine : IHommEngine
    {
        public LogWriter LogWriter { get; set; }

        private BaseHommEngine engine;

        public HommEngine(bool fancyGraphics)
        {
            engine = fancyGraphics ? new FancyHommEngine() : new SimpleHommEngine() as BaseHommEngine;
        }

        [ToLog]
        public void CreateObject(string id, MapObject mapObject, int x = 0, int y = 0)
        {
            this.Log($"{nameof(CreateObject)}", id, mapObject, x, y);
            engine.CreateObject(id, mapObject, x, y);
        }

        [ToLog]
        public void Move(string id, Direction direction, double duration)
        {
            this.Log($"{nameof(Move)}", id, direction, duration);
            engine.Move(ObjectsCache.FindGameObject(id), direction, (float)duration);
        }

        [ToLog]
        public void SetPosition(string id, int x, int y)
        {
            this.Log($"{nameof(SetPosition)}", id, x, y);
            engine.SetPosition(ObjectsCache.FindGameObject(id), x, y);
        }

        [ToLog]
        public void SetFlag(string id, string ownerId)
        {
            this.Log($"{nameof(SetFlag)}", id, ownerId);
            engine.SetFlag(ObjectsCache.FindGameObject(id), ownerId);
        }

        [ToLog]
        public void SetUpScene(int width, int height)
        {
            this.Log($"{nameof(SetUpScene)}", width, height);
            engine.SetUpScene(width, height);
        }

        [ToLog]
        public void Freeze(string id)
        {
            this.Log($"{nameof(Freeze)}", id);
            engine.Freeze(ObjectsCache.FindGameObject(id));
        }

        [ToLog]
        public void SetColor(string id, float r, float g, float b)
        {
            this.Log($"{nameof(SetColor)}", id, r, g, b);
            engine.SetColor(ObjectsCache.FindGameObject(id), new Color(r, g, b));
        }

        [ToLog]
        public void SetRotation(string id, float angleRad)
        {
            this.Log($"{nameof(SetRotation)}", id, angleRad);
            engine.SetRotation(ObjectsCache.FindGameObject(id), angleRad);
        }

        [ToLog]
        public void SetAnimation(string id, Animation animation)
        {
            this.Log($"{nameof(SetAnimation)}", id, animation);
            engine.SetAnimation(ObjectsCache.FindGameObject(id), animation);
        }
    }
}
