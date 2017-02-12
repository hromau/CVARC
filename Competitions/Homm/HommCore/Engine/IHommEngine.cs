using CVARC.V2;
using UnityEngine;

namespace HoMM.Engine
{
    public interface IHommEngine : IEngine
    {
        void SetUpScene(int width, int height);
        void CreateObject(string id, MapObject obj, int x, int y);
        void Move(string thingId, Direction direction, double duration);
        void Freeze(string thingId);
        void SetPosition(string thingId, int x, int y);
        void SetRotation(string thingId, float angleRadians);
        void SetColor(string thingId, float r, float g, float b);
        void SetFlag(string thingId, string ownerId);
        void SetAnimation(string thingId, Animation animation);
    }
}
