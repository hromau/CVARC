using System;
using AIRLab.Mathematics;

namespace Pudge.ClientClasses
{
    [Serializable]
    public class RuneData
    {
        public readonly Point2D Location;
        public readonly RuneType Type;
        public readonly RuneSize Size;

        public RuneData(RuneType type, RuneSize size, Point2D location = new Point2D())
        {
            Location = location;
            Type = type;
            Size = size;
        }
    }
}