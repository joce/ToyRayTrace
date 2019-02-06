using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ToyRayTrace
{
    static class Rng
    {
        static readonly Random k_Rng = new Random(0);// We always want the same results every runs.

        // Not thread safe!
        public static float Next()
        {
            return (float)k_Rng.NextDouble();
        }

#if false

        public static float Next(ref uint _)
        {
            return (float)k_Rng.NextDouble();
        }
#else

        static uint XorShift32(ref uint state)
        {
            uint x = state;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 15;
            state = x;
            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Next(ref uint state)
        {
            return (XorShift32(ref state) & 0xFFFFFF) / 16777216.0f;
        }
#endif

        public static Vector3 NextInUnitSphere(ref uint state)
        {
            Vector3 p;
            do
            {
                p = new Vector3(2 * Next(ref state) - 1, 2 * Next(ref state) - 1, 2 * Next(ref state) - 1);
            } while (p.LengthSquared() >= 1f);

            return p;
        }

        public static Vector3 NextInUnitDisc(ref uint state)
        {
            Vector3 p;
            do
            {
                p = new Vector3(2 * Next(ref state) - 1, 2 * Next(ref state) - 1, 0);
            } while (p.LengthSquared() >= 1f);

            return p;
        }
    }
}
