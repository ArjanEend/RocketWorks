﻿using System;

namespace RocketWorks
{
    [Serializable]
    public partial struct Vector2
    {
        private const float Eps = 1e-7f;

        public static Vector2 zero = new Vector2();
        public static Vector2 up = new Vector2(0, 1);

        public float x;
        public float y;

        public Vector2(float x = 0f, float y = 0f)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 Set(float x, float y)
        {
            this.x = x;
            this.y = y;
            return this;
        }

        public static Vector2 Lerp(Vector2 position1, Vector2 position2, float deltaTime)
        {
            deltaTime = Mathf.Clamp(deltaTime, 0f, 1f);
            return new Vector2(position1.x + deltaTime * (position2.x - position1.x), position1.y + deltaTime * (position2.y - position1.y));
        }

        public static float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (pos1 - pos2).Magnitude();
        }

        public float Magnitude()
        {
            return Mathf.Sqrt(x * x + y * y);
        }
        public void Normalize()
        {
            float length = Magnitude();
            if (length == 0f)
                return;
            x /= length;
            y /= length;
        }
        public Vector2 Normalized()
        {
            float length = Magnitude();
            if (length == 0f)
                return Vector2.zero;
            return new Vector2(x / length, y / length);
        }
        public float Angle()
        {
            return Mathf.Atan2(x, y);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static Vector2 operator +(Vector2 a, float b)
        {
            return new Vector2(a.x + b, a.y + b);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - b.x, a.y - b.y);
        }

        public static Vector2 operator -(Vector2 a, float b)
        {
            return new Vector2(a.x - b, a.y - b);
        }

        public static Vector2 operator -(float a, Vector2 b)
        {
            return new Vector2(a - b.x, a - b.y);
        }

        public static Vector2 operator -(Vector2 b)
        {
            return new Vector2(-b.x, -b.y);
        }

        public static Vector2 operator *(float d, Vector2 v)
        {
            return new Vector2(d * v.x, d * v.y);
        }

        public static Vector2 operator *(Vector2 v, float d)
        {
            return new Vector2(v.x * d, v.y * d);
        }

        public static Vector2 operator /(Vector2 v, float d)
        {
            return new Vector2(v.x / d, v.y / d);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            if (ReferenceEquals(a, null))
            {
                return ReferenceEquals(b, null);
            }
            if (ReferenceEquals(b, null))
            {
                return false;
            }
            return Math.Abs(a.x - b.x) < Eps && Math.Abs(a.y - b.y) < Eps;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Vector2 other = (Vector2)obj;
            return other != null && Math.Abs(other.x - y) < Eps && Math.Abs(other.y - y) < Eps;
        }
        public float Dot(Vector2 other)
        {
            return x * other.x + y * other.y;
        }

        public Vector2 Intersection(Vector2 movement, Vector2 other, Vector2 otherMovement)
        {
            if((x == other.x && y == other.y) || (x == -other.x && y == -other.y))
                return Vector2.zero;

            Vector2 difference = other - this;

            float t = difference.Dot(otherMovement) / movement.Perp(otherMovement);

            return new Vector2(x + movement.x * t, y + movement.y * t);
        }

        public float Perp(Vector2 other)
        {
            return x * other.y - y * other.x;
        }

        public void Rotate(float degrees)
        {
            degrees %= 360f;
            float angle = degrees * Mathf.PI / 180;

            float tempX = x;
            float tempY = y;

            x = tempX * Mathf.Cos(angle) - tempY * Mathf.Sin(angle);
            y = tempY * Mathf.Cos(angle) - tempX * Mathf.Sin(angle);
        }
        
        public void Project(Vector2 other)
        {
            float dot = Dot(other);
            x = dot * other.x;
            y = dot * other.y;
        }

        public override string ToString()
        {
            return string.Format("x: {0}, y: {1}", x, y);
        }

        public static implicit operator Vector2(Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

#if UNITY_EDITOR || UNITY_5
        public static implicit operator UnityEngine.Vector2(Vector2 v)
        {
            return new UnityEngine.Vector2(v.x, v.y);
        }
        public static implicit operator Vector2(UnityEngine.Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }
#endif
    }
}
