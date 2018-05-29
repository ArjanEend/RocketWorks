
using System;

namespace RocketWorks
{
    public partial struct Vector3
    {
        private const float Eps = 1e-7f;

        public float x;
        public float y;
        public float z;


        public static Vector3 zero = new Vector3();
        public static Vector3 one = new Vector3(1, 1, 1);

        public Vector3(float x = 0f, float y = 0f, float z = 0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 Set(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            return this;
        }

        public static Vector3 Lerp(Vector3 position1, Vector3 position2, float deltaTime)
        {
            deltaTime = Mathf.Clamp(deltaTime, 0f, 1f);
            return new Vector3(position1.x + deltaTime * (position2.x - position1.x), position1.y + deltaTime * (position2.y - position1.y), position1.z+ deltaTime * (position2.z - position1.z));
        }

        public static float Distance(Vector3 pos1, Vector3 pos2)
        {
            return (pos1 - pos2).Magnitude();
        }

        public float Magnitude()
        {
            return Mathf.Sqrt((x * x) + (y * y) + (z * z));
        }

        public Vector3 Normalized()
        {
            float length = Magnitude();
            if (length == 0f)
                return Vector3.zero;
            return new Vector3(x / length, y / length, z / length);
        }

        public void Normalize()
        {

            float length = Magnitude();
            if (length == 0f)
                return;
            x /= length;
            y /= length;
            z /= length;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator +(Vector3 a, float b)
        {
            return new Vector3(a.x + b, a.y + b, a.z + b);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator -(Vector3 a, float b)
        {
            return new Vector3(a.x - b, a.y - b, a.z - b);
        }

        public static Vector3 operator -(float a, Vector3 b)
        {
            return new Vector3(a - b.x, a - b.y, a - b.z);
        }

        public static Vector3 operator -(Vector3 b)
        {
            return new Vector3(-b.x, -b.y, -b.z);
        }

        public static Vector3 operator *(float d, Vector3 v)
        {
            return new Vector3(d * v.x, d * v.y, d * v.z);
        }

        public static Vector3 operator *(Vector3 v, float d)
        {
            return new Vector3(v.x * d, v.y * d, v.z * d);
        }

        public static Vector3 operator /(Vector3 v, float d)
        {
            return new Vector3(v.x / d, v.y / d, v.z / d);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return false;
            }
            return Math.Abs(a.x - b.x) < Eps && Math.Abs(a.y - b.y) < Eps && Math.Abs(a.z - b.z) < Eps;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Vector3 other = (Vector3)obj;
            return other != null && Math.Abs(other.x - y) < Eps && Math.Abs(other.y - y) < Eps && Math.Abs(other.z - z) < Eps;
        }
        public float Dot(Vector3 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }
        
        public static implicit operator Vector3(Vector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }

#if RW_UNITY
        public static implicit operator UnityEngine.Vector3(Vector3 v)
        {
            return new UnityEngine.Vector3(v.x, v.y, v.z);
        }
        public static implicit operator Vector3(UnityEngine.Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
#endif

    }
}
