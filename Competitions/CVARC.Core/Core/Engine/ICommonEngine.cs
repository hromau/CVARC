using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using CVARC.Basic.Sensors;

namespace CVARC.V2
{
    public interface ICommonEngine : IEngine
    {
        void SetAbsoluteSpeed(string id, Frame3D speed);
        void SetRelativeSpeed(string id, Frame3D speed);
        Frame3D GetSpeed(string id);
        Frame3D GetAbsoluteLocation(string id);
        bool ContainBody(string id);

        event Action<string, string> Collision;
        void Attach(string objectToAttach, string host, Frame3D relativePosition);
        void Detach(string objectToDetach, Frame3D absolutePosition);
        string FindParent(string objectId);

        void DeleteObject(string objectId);
        void SetAbsoluteLocation(string id, Frame3D location);
    }


    public static class ICommonEngineExtensions
    {
        public static bool IsAttached(this ICommonEngine engine, string objectId)
        {
            return engine.FindParent(objectId) != null;
        }
    }
}
