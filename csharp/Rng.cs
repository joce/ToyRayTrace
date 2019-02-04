using System;

namespace ToyRayTrace
{
    static class Rng
    {
        static Random s_Rng = new Random(0); // We always want the same results every runs.

        public static float Next() => (float)s_Rng.NextDouble();

        public static Vec3 NextInUnitSphere()
        {
            Vec3 p;
            do
            {
                p = new Vec3(2 * Next() - 1, 2 * Next() - 1, 2 * Next() - 1);
            } while (p.SquaredLength >= 1f);

            return p;
        }

        public static Vec3 NextInUnitDisc()
        {
            Vec3 p;
            do
            {
                p = new Vec3(2 * Next() - 1, 2 * Next() - 1, 0);
            } while (p.SquaredLength >= 1f);

            return p;
        }
    }
}
