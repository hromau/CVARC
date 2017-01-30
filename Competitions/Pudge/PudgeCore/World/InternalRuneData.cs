using System;
using System.Collections.Generic;
using AIRLab.Mathematics;

namespace Pudge.World
{
    [Serializable]
    public class InternalRuneData
    {
        public readonly RuneType Type;
        public readonly RuneSize Size;
        public readonly Frame3D Location;

        public InternalRuneData(RuneType type, RuneSize size,  Frame3D location=new Frame3D())
        {
            Type = type;
            Size = size;
            Location = location;
        }
        protected bool Equals(InternalRuneData other)
        {
            return Type == other.Type && Size == other.Size && Location.Equals(other.Location);
        }
        public class EqualityComparer : IEqualityComparer<InternalRuneData>
        {
            public bool Equals(InternalRuneData data1, InternalRuneData data2)
            {
                return data1.Type == data2.Type && data1.Size == data2.Size;
            }

            public int GetHashCode(InternalRuneData data)
            {
                unchecked
                {
                    return ((int) data.Type * 397) ^ (int) data.Size;
                }
            }
        }
    }
}
