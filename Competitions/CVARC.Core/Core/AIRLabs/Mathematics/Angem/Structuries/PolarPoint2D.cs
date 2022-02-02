
using System;
using Newtonsoft.Json;

namespace AIRLab.Mathematics
{
    public struct PolarPoint2D
    {
        [JsonConstructor]
        public PolarPoint2D(double r, Angle angle) : this()
        {
            Alpha = angle;
            R = r;
        }

        public double R { get; private set; }
        public Angle Alpha { get; private set; }

        public Point2D ToPoint2D()
        {
            return new Point2D(R*Math.Cos(Alpha.Radian), R*Math.Sin(Alpha.Radian));
        }
    }
}