using System;

namespace ToyRayTrace
{
    public class Dielectric : IMaterial
    {
        readonly float m_RefIdx;
        readonly float m_R0;

        public Dielectric(float refIdx)
        {
            m_RefIdx = refIdx;
            m_R0 = (1.0f - m_RefIdx) / (1.0f + m_RefIdx);
            m_R0 = m_R0 * m_R0;
        }

        public bool Scatter(in Ray inRay, HitRecord rec, ref uint state, out Vec3 attenuation, out Ray scattered)
        {
            Vec3 outwardNormal;
            float niOverNt;
            float reflectProb;
            float cosine;
            if (Vec3.Dot(inRay.Direction, rec.normal) > 0)
            {
                outwardNormal = -rec.normal;
                niOverNt = m_RefIdx;
                cosine = Vec3.Dot(inRay.Direction, rec.normal) / inRay.Direction.Length;
                cosine = MathF.Sqrt(1 - m_RefIdx*m_RefIdx*(1-cosine*cosine));
            }
            else
            {
                outwardNormal = rec.normal;
                niOverNt = 1.0f / m_RefIdx;
                cosine = -Vec3.Dot(inRay.Direction, rec.normal) / inRay.Direction.Length;
            }

            if (Vec3.Refract(inRay.Direction, outwardNormal, niOverNt, out var refracted))
                reflectProb = Schlick(cosine);
            else
                reflectProb = 1.0f;

            attenuation = Vec3.One;
            if (Rng.Next(ref state) < reflectProb)
            {
                var reflected = Vec3.Reflect(inRay.Direction, rec.normal);
                scattered = new Ray(rec.p, reflected);
            }
            else
                scattered = new Ray(rec.p, refracted);

            return true;
        }

        float Schlick(float cosine)
        {
//            var r0 = (1.0f - m_RefIdx) / (1.0f + m_RefIdx);
//            r0 = r0 * r0;
            return m_R0 + (1.0f - m_R0) * MathF.Pow(1.0f - cosine, 5);
        }
    }
}
