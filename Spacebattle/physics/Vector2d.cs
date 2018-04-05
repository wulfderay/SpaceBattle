using Spacebattle.Extensions;
using System;
using System.Data.SqlTypes;

namespace Spacebattle.physics
{
    public class Vector2d
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static Vector2d Zero => new Vector2d {X = 0, Y = 0};

        public Vector2d(){}

        public Vector2d(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float DistanceTo(Vector2d other)
        {
            var x = (X - other.X);
            var y = (Y - other.Y);
            return (float)Math.Sqrt((x * x) + (y * y));
        }

        public float DirectionInRadiansTo(Vector2d other)
        {
            var difference = other - this;
            return (float)((2 * Math.PI)+ Math.Atan2(difference.Y, difference.X) % (2 * Math.PI));
        }

        public float DirectionInDegreesTo(Vector2d other)
        {
            var difference = other - this;
            return (360 + (float)(Math.Atan2(difference.Y, difference.X)).ToDegrees()) % 360;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt((X *X) + (Y * Y));
        }

        public static Vector2d fromAngleDegrees(float angleInDegrees)
        {
            return new Vector2d {
                X = (float)Math.Cos(angleInDegrees.ToRadians()),
                Y = (float)Math.Sin(angleInDegrees.ToRadians())
            };
        }

        public static Vector2d fromAngleRadains(float angleInRadians)
        {
            return new Vector2d
            {
                X = (float)Math.Cos(angleInRadians),
                Y = (float)Math.Sin(angleInRadians)
            };
        }

        public static Vector2d operator /(Vector2d vector2d, float scalar)
        {
            vector2d.X /= scalar;
            vector2d.Y /= scalar;
            return vector2d;
        }

        public static Vector2d operator *(Vector2d vector2d, float scalar)
        {
            var x = vector2d.X * scalar;
            var y = vector2d.Y * scalar;

            return new Vector2d(x,y);
        }

        public static Vector2d operator +(Vector2d vector2d, Vector2d other)
        {
            var x = vector2d.X + other.X;
            var y = vector2d.Y + other.Y;

            return new Vector2d(x, y);
        }

        public static Vector2d operator -(Vector2d vector2d, Vector2d other)
        {
            var x = vector2d.X - other.X;
            var y = vector2d.Y - other.Y;

            return new Vector2d(x, y);
        }
    }
}
