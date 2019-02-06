using System;
using System.Numerics;

namespace ToyRayTrace
{
    public struct Metal : IMaterial
    {
        readonly Vector3 m_Albedo;
        readonly float m_Fuzziness;

        public Metal(in Vector3 albedo, float fuzziness = 0)
        {
            m_Albedo = albedo;
            m_Fuzziness = fuzziness < 0 ? 0 : fuzziness > 1 ? 1 : fuzziness;
        }

        public bool Scatter(in Ray inRay, HitRecord rec, ref uint state, out Vector3 attenuation, out Ray scattered)
        {
            var reflected = Vector3.Reflect(inRay.Direction, rec.normal);
            scattered = new Ray(rec.p, reflected + Rng.NextInUnitSphere(ref state) * m_Fuzziness);
            attenuation = m_Albedo;
            return Vector3.Dot(scattered.Direction, rec.normal) > 0;
        }
    }
}
