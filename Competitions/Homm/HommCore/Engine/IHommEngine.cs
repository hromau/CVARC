using CVARC.V2;
using UnityEngine;

namespace HoMM.Engine
{
    public interface IHommEngine : IEngine
    {
        void CreateObject(string id, MapObject obj, int x, int y);
        void Move(string thingId, Direction direction, double duration);
        void Freeze(string thingId);
        void SetColor(string thingId, float r, float g, float b);
        void SetScale(string thingId, float x, float y, float z);
        void SetPosition(string thingId, int x, int y);
        void SetFlag(string thingId, float r, float g, float b);
        void SetCameraPosition(float x, float y, float z);
        void SetCameraRotation(float x, float y, float z);
    }
}
