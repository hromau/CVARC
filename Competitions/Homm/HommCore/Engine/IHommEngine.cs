using CVARC.V2;
using UnityEngine;


namespace HoMM.Engine {
    public interface IHommEngine : IEngine {
        GameObject CreateObject(string id, MapObject obj, int x, int y);
        void SetPosition(string id, int x, int y);
        void Move(string id, Direction direction, double duration);
        void SetFlag(string id, string player);
        void DeleteObject(string id);
    }
}
