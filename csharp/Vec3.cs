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

        readonly float[] e;

        public float X => e[0];
        public float Y => e[1];
        public float Z => e[2];

        public float R => e[0];
        public float G => e[1];
        public float B => e[2];

        public Vec3(float v1, float v2, float v3)
        {
            e = new float[3];
            e[0] = v1;
            e[1] = v2;
            e[2] = v3;
        }

        public Vec3(in Vec3 v)
        {
            e = new float[3];
            e[0] = v.e[0];
            e[1] = v.e[1];
            e[2] = v.e[2];
        }

        public float this[int i] => e[i];

        public float Length => MathF.Sqrt(e[0] * e[0] + e[1] * e[1] + e[2] * e[2]);
        public float SquaredLength => e[0] * e[0] + e[1] * e[1] + e[2] * e[2];
        public bool IsNormalized => Math.Abs(SquaredLength - 1.0f) < 0.01f;

        public static Vec3 operator +(in Vec3 v) => new Vec3(v);
        public static Vec3 operator -(in Vec3 v) => new Vec3(-v.e[0], -v.e[1], -v.e[2]);
        public static Vec3 operator +(in Vec3 a, in Vec3 b) => new Vec3(a.e[0] + b.e[0], a.e[1] + b.e[1], a.e[2] + b.e[2]);
        public static Vec3 operator -(in Vec3 a, in Vec3 b) => new Vec3(a.e[0] - b.e[0], a.e[1] - b.e[1], a.e[2] - b.e[2]);
        public static Vec3 operator *(in Vec3 a, in Vec3 b) => new Vec3(a.e[0] * b.e[0], a.e[1] * b.e[1], a.e[2] * b.e[2]);
        public static Vec3 operator /(in Vec3 a, in Vec3 b) => new Vec3(a.e[0] / b.e[0], a.e[1] / b.e[1], a.e[2] / b.e[2]);
        public static Vec3 operator *(in Vec3 v, in float f) => new Vec3(v.e[0] * f, v.e[1] * f, v.e[2] * f);
        public static Vec3 operator /(in Vec3 v, float f) => new Vec3(v.e[0] / f, v.e[1] / f, v.e[2] / f);
        public static Vec3 operator *(float f, in Vec3 v) => new Vec3(v.e[0] * f, v.e[1] * f, v.e[2] * f);

        public static Vec3 Normalize(in Vec3 v)
        {
            var k = 1 / v.Length;
            return new Vec3(v.e[0]*k, v.e[1]*k, v.e[2]*k);
        }

        public static float Dot(in Vec3 a, in Vec3 b) => a.e[0] * b.e[0] + a.e[1] * b.e[1] + a.e[2] * b.e[2];

        public static Vec3 Cross(in Vec3 a, in Vec3 b)
        {
            return new Vec3(
                a.e[1]*b.e[2] - a.e[2]*b.e[1],
                -(a.e[0]*b.e[2] - a.e[2]*b.e[0]),
                a.e[0]*b.e[1] - a.e[1]*b.e[0]);
        }

        public static Vec3 Reflect(in Vec3 v, in Vec3 normal)
        {
            Debug.Assert(v.IsNormalized);
            return v - 2 * Dot(v, normal) * normal;
        }

        public static bool Refract(in Vec3 v, in Vec3 normal, float niOverNt, out Vec3 refracted)
        {
            Debug.Assert(v.IsNormalized);
            var dt = Dot(v, normal);
            var discriminant = 1.0f - niOverNt * niOverNt * (1 - dt * dt);
            if (discriminant > 0)
            {
                refracted = niOverNt * (v - normal* dt) - normal * MathF.Sqrt(discriminant);
                Debug.Assert(refracted.IsNormalized);
                return true;
            }
            refracted = Zero;
            return false;
        }
    }
}
