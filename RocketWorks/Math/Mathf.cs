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
    }
}
