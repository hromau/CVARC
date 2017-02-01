using System;
using System.Collections.Generic;

namespace HoMM
{
    public class Location : Vector2i
    {
        public static readonly Location Zero = new Location(0, 0);

        public Location(int y, int x) : base(x, y) { }

        public IEnumerable<Location> Neighborhood
        {
            get
            {
                for (var dy = -1; dy < 2; dy++)
                    for (var dx = -1; dx < 2; dx++)
                        if (dx * dx + dy * dy != 0 && dy * dx * dx != 1)
                            yield return new Location(Y + dy + (X % 2) * dx * dx, X + dx);
            }
        }

        public Location DiagonalMirror(MapSize size)
        {
            return IsOnDiagonal(size) 
                ? X < size.X / 2 ? this : new Location(size.Y - Y - 1, size.X - X - 1)
                : new Location(size.Y - Y - 1, size.X - X - 1);
        }

        public bool IsInside(MapSize size)
        {
            return X >= 0 && X < size.X && Y >= 0 && Y < size.Y;
        }

        public bool IsAboveDiagonal(MapSize size)
        {
            return Y < size.Y - (float)X / size.X * size.Y - 1;
        }

        public bool IsBelowDiagonal(MapSize size)
        {
            return Y > size.Y - (float)X / size.X * size.Y - 1;
        }

        public bool IsOnDiagonal(MapSize size)
        {
            return !IsAboveDiagonal(size) && !IsBelowDiagonal(size);
        }

        public Location AboveDiagonal(MapSize size)
        {
            return IsAboveDiagonal(size) ? this : DiagonalMirror(size);
        }

        public static IEnumerable<Location> Square(MapSize size)
        {
            for (int y = 0; y < size.Y; ++y)
                for (int x = 0; x < size.X; ++x)
                    yield return new Location(y, x);
        }

        public double EuclideanDistance(Location other)
        {
            var thisFixY = Y + 0.5 * (X % 2);
            var otherFixY = other.Y + 0.5 * (other.X % 2);
            return Math.Sqrt(Math.Pow(X-other.X, 2) + Math.Pow(thisFixY-otherFixY, 2));
        }

        public double ManhattanDistance(Location other)
        {
            var thisFixY = Y + 0.5 * (X % 2);
            var otherFixY = other.Y + 0.5 * (other.X % 2);
            return Math.Abs(X-other.X) + Math.Abs(thisFixY-otherFixY);
        }

        public Location NeighborAt(Direction Direction)
        {
            var isEven = Y % 2 == 0;

            switch (Direction)
            {
                case Direction.Up:
                    return new Location(Y - 1, X);

                case Direction.Down:
                    return new Location(Y + 1, X);

                case Direction.LeftUp:
                    return new Location(isEven ? Y - 1 : Y, X - 1);

                case Direction.LeftDown:
                    return new Location(isEven ? Y : Y + 1, X - 1);

                case Direction.RightUp:
                    return new Location(isEven ? Y - 1 : Y, X + 1);

                case Direction.RightDown:
                    return new Location(isEven ? Y : Y + 1, X + 1);
            }

            throw new ArgumentException($"Value of {nameof(Direction)} is invalid" +
                " and not supported by this method!");
        }
    }
}
