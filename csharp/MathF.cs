using System;

namespace ToyRayTrace
{
    public static class MathF
    {
        public static float Sqrt(float v) => (float)Math.Sqrt(v);
        public static float Pow(float a, float b) => (float)Math.Pow(a, b);
        public static float Sin(float v) => (float)Math.Sin(v);
        public static float Cos(float v) => (float)Math.Cos(v);
        public static float Tan(float v) => (float)Math.Tan(v);
        public static float Max(float a, float b) => Math.Max(a, b);
        public const float PI = (float)Math.PI;
    }
}
