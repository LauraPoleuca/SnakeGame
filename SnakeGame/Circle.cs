using System;

namespace SnakeGame
{
    class Circle
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Circle()
        {
            X = -1;
            Y = -1;
        }

        public override bool Equals(object obj)
        {
            return obj is Circle circle &&
                   X == circle.X &&
                   Y == circle.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
