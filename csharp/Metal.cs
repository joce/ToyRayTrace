using System;

namespace ToyRayTrace
{
    public struct Metal : IMaterial
    {
        readonly Vec3 m_Albedo;
        readonly float m_Fuzziness;

        public Metal(in Vec3 albedo, float fuzziness = 0)
        {
            m_Albedo = albedo;
            m_Fuzziness = fuzziness < 0 ? 0 : fuzziness > 1 ? 1 : fuzziness;
        }

        public bool Scatter(in Ray inRay, HitRecord rec, ref uint state, out Vec3 attenuation, out Ray scattered)
        {
            var reflected = Vec3.Reflect(inRay.Direction, rec.normal);
            scattered = new Ray(rec.p, reflected + Rng.NextInUnitSphere(ref state) * m_Fuzziness);
            attenuation = m_Albedo;
            return Vec3.Dot(scattered.Direction, rec.normal) > 0;
        }
    }
}
