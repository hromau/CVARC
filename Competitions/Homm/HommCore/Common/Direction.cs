using System;

namespace HoMM
{
    public enum Direction
    {
        Up,
        Down,
        LeftUp,
        LeftDown,
        RightUp,
        RightDown
    }

    public static class DirectionExtensions
    {
        public static float ToUnityAngle(this Direction direction)
        {
            Func<float, float> rad = deg => deg * (float)Math.PI / 180;

            switch (direction)
            {
                case Direction.Down: return rad(90);
                case Direction.LeftDown: return rad(90 + 60);
                case Direction.LeftUp: return rad(90 + 60 * 2);
                case Direction.Up: return rad(90 + 60 * 3);
                case Direction.RightUp: return rad(90 + 60 * 4);
                case Direction.RightDown: return rad(90 + 60 * 5);

                default: throw new ArgumentException();
            }

        }
    }
}
