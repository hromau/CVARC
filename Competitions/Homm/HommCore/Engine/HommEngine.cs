using CVARC.V2;
using UnityCommons;
using UnityEngine;

namespace HoMM.Engine
{
    public class HommEngine : IHommEngine
    {
        public LogWriter LogWriter { get; set; }

        private InternalHommEngine engine = new InternalHommEngine();

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
            engine.Move(ObjectsCache.FindGameObject(id), direction, duration);
        }

        [ToLog]
        public void SetColor(string id, float r, float g, float b)
        {
            this.Log($"{nameof(SetColor)}", id, r, g, b);
            engine.SetColor(ObjectsCache.FindGameObject(id), new Color(r, g, b));
        }

        [ToLog]
        public void SetPosition(string id, int x, int y)
        {
            this.Log($"{nameof(SetPosition)}", id, x, y);
            engine.SetPosition(ObjectsCache.FindGameObject(id), x, y);
        }

        [ToLog]
        public void SetScale(string id, float x, float y, float z)
        {
            this.Log($"{nameof(SetScale)}", id, x, y, z);
            engine.SetScale(ObjectsCache.FindGameObject(id), x, y, z);
        }

        [ToLog]
        public void SetFlag(string id, float r, float g, float b)
        {
            this.Log($"{nameof(SetFlag)}", id, r, g, b);
            engine.SetFlag(ObjectsCache.FindGameObject(id), new Color(r, g, b));
        }

        public void SetCameraPosition(float x, float y, float z)
        {
            this.Log($"{nameof(SetCameraPosition)}", x, y, z);
            engine.SetCameraPosition(new Vector3(x, y, z));
        }

        public void SetCameraRotation(float x, float y, float z)
        {
            this.Log($"{nameof(SetCameraRotation)}", x, y, z);
            engine.SetCameraRotation(Quaternion.Euler(x, y, z));
        }
    }
}
