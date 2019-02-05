using System;
using System.Diagnostics;

namespace ToyRayTrace
{
    public readonly struct Vec3
    {
        static readonly Vec3 k_Zero = new Vec3(0, 0, 0);
        static readonly Vec3 k_One = new Vec3(1, 1, 1);
        public static ref readonly Vec3 Zero => ref k_Zero;
        public static ref readonly Vec3 One => ref k_One;

        public readonly float x, y, z;

        public Vec3(float v1, float v2, float v3)
        {
            x = v1;
            y = v2;
            z = v3;
        }

        public Vec3(in Vec3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public float Length => MathF.Sqrt(x * x + y * y + z * z);
        public float SquaredLength => x * x + y * y + z * z;
        public bool IsNormalized => Math.Abs(SquaredLength - 1.0f) < 0.01f;

        public static Vec3 operator +(in Vec3 v) => new Vec3(v);
        public static Vec3 operator -(in Vec3 v) => new Vec3(-v.x, -v.y, -v.z);
        public static Vec3 operator +(in Vec3 a, in Vec3 b) => new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vec3 operator -(in Vec3 a, in Vec3 b) => new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vec3 operator *(in Vec3 a, in Vec3 b) => new Vec3(a.x * b.x, a.y * b.y, a.z * b.z);
        public static Vec3 operator /(in Vec3 a, in Vec3 b) => new Vec3(a.x / b.x, a.y / b.y, a.z / b.z);
        public static Vec3 operator *(in Vec3 v, float f) => new Vec3(v.x * f, v.y * f, v.z * f);
        public static Vec3 operator /(in Vec3 v, float f) => new Vec3(v.x / f, v.y / f, v.z / f);

        public static Vec3 Normalize(in Vec3 v)
        {
            var k = 1 / v.Length;
            return new Vec3(v.x*k, v.y*k, v.z*k);
        }

        public static float Dot(in Vec3 a, in Vec3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        public static Vec3 Cross(in Vec3 a, in Vec3 b)
        {
            return new Vec3(
                a.y*b.z - a.z*b.y,
                -(a.x*b.z - a.z*b.x),
                a.x*b.y - a.y*b.x);
        }

        public static Vec3 Reflect(in Vec3 v, in Vec3 normal)
        {
            Debug.Assert(v.IsNormalized);
            return v - normal * 2 * Dot(v, normal);
        }

        public static bool Refract(in Vec3 v, in Vec3 normal, float niOverNt, out Vec3 refracted)
        {
            Debug.Assert(v.IsNormalized);
            var dt = Dot(v, normal);
            var discriminant = 1.0f - niOverNt * niOverNt * (1 - dt * dt);
            if (discriminant > 0)
            {
                refracted = (v - normal* dt) * niOverNt - normal * MathF.Sqrt(discriminant);
                Debug.Assert(refracted.IsNormalized);
                return true;
            }
            refracted = Zero;
            return false;
        }
    }
}
