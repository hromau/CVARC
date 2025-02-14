﻿namespace HoMM.ClientClasses
{
    public class LocationInfo
    {
        public int X { get; set; }
        public int Y { get; set; }

        public LocationInfo(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Location ToLocation()
        {
            return new Location(Y, X);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
