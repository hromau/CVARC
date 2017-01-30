using System;

namespace Pudge.Units.DaggerUnit
{
    [Serializable]
    public class DaggerDestinationPoint
    {
        public DaggerDestinationPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X{ get; set; }
        public int Y{ get; set; }
    }
}