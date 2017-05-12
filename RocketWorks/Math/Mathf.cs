using System;

namespace RocketWorks
{
    public class Mathf
    {
        public static float PI
        {
            get { return (float)Math.PI; }
        }
        public static float Atan2(float x, float y)
        {
            return (float)Math.Atan2(x, y);
        }
        public static float Sqrt(float x)
        {
            return (float)Math.Sqrt(x);
        }
        public static float Sin(float x)
        {
            return (float)Math.Sin(x);
        }
        public static float Cos(float x)
        {
            return (float)Math.Cos(x);
        }

        public static int RoundToInt(float v)
        {
            return (int)Math.Round(v);
        }

        public static float Clamp(float value, float v1, float v2)
        {
            return Math.Min(Math.Max(value, v1), v2);
        }

        internal static float Abs(float v)
        {
            return Math.Abs(v);
        }
    }
}
