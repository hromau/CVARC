using AIRLab.Mathematics;

namespace Pudge.Sensors.Map
{
    public enum HeroType
    {
        Pudge,
        Slardar
    }

    public class HeroData
    {
        public Frame3D Location{ get; set; }
        public HeroType Type{ get; set; }
    }
}