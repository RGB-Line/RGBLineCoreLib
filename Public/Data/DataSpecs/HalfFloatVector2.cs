using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RGBLineCoreLib.Data
{
    public struct HalfFloatVector2
    {
        public float X;
        public int Y;


        public static HalfFloatVector2 one
        {
            get
            {
                return new HalfFloatVector2()
                {
                    X = 1.0f,
                    Y = 1
                };
            }
        }
        public static HalfFloatVector2 zero
        {
            get
            {
                return new HalfFloatVector2()
                {
                    X = 0.0f,
                    Y = 0
                };
            }
        }

        public static bool operator == (HalfFloatVector2 vector1, HalfFloatVector2 vector2)
        {
            return vector1.X == vector2.X && vector1.Y == vector2.Y;
        }
        public static bool operator !=(HalfFloatVector2 vector1, HalfFloatVector2 vector2)
        {
            return vector1.X != vector2.X || vector1.Y != vector2.Y;
        }

        public static HalfFloatVector2 operator -(HalfFloatVector2 vector)
        {
            return new HalfFloatVector2()
            {
                X = -vector.X,
                Y = -vector.Y
            };
        }

        public static HalfFloatVector2 operator +(HalfFloatVector2 vector1, HalfFloatVector2 vector2)
        {
            return new HalfFloatVector2()
            {
                X = vector1.X + vector2.X,
                Y = vector1.Y + vector2.Y
            };
        }
        public static HalfFloatVector2 operator -(HalfFloatVector2 vector1, HalfFloatVector2 vector2)
        {
            return new HalfFloatVector2()
            {
                X = vector1.X - vector2.X,
                Y = vector1.Y - vector2.Y
            };
        }
        public static HalfFloatVector2 operator *(HalfFloatVector2 vector, float scalar)
        {
            return new HalfFloatVector2()
            {
                X = vector.X * scalar,
                Y = (int)(vector.Y * scalar)
            };
        }
        public static HalfFloatVector2 operator /(HalfFloatVector2 vector, float scalar)
        {
            return new HalfFloatVector2()
            {
                X = vector.X / scalar,
                Y = (int)(vector.Y / scalar)
            };
        }
    }
}