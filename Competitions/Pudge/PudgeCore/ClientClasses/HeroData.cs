using System;
using AIRLab.Mathematics;
using Pudge.Sensors.Map;

namespace Pudge.ClientClasses
{
    [Serializable]
    public class HeroData
    {
        public readonly Point2D Location;
        public readonly double Angle;
        public readonly HeroType Type;
        public HeroData(HeroType type, Frame3D location = new Frame3D())
        {
            Location = location.ToFrame2D().ToPoint2D();
            Type = type;
            Angle = location.Yaw.Simplify360().Grad;
        }
    }

    [Serializable]
    public class HookData
    {
        public readonly Point2D Location;

        public HookData(Frame3D location)
        {
            Location = location.ToFrame2D().ToPoint2D();
        }
    }
}