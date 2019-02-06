using System;
using System.Diagnostics;
using System.Numerics;

namespace ToyRayTrace
{
    public class Dielectric : IMaterial
    {
        readonly float m_RefIdx;
        readonly float m_R0;

        static bool Refract(in Vector3 v, in Vector3 normal, float niOverNt, out Vector3 refracted)
        {
            Debug.Assert(Math.Abs(v.LengthSquared() - 1) < 0.001);
            var dt = Vector3.Dot(v, normal);
            var discriminant = 1.0f - niOverNt * niOverNt * (1 - dt * dt);
            if (discriminant > 0)
            {
                refracted = (v - normal* dt) * niOverNt - normal * MathF.Sqrt(discriminant);
                Debug.Assert(Math.Abs(refracted.LengthSquared() - 1) < 0.001);
                return true;
            }
            refracted = Vector3.Zero;
            return false;
        }

        public Dielectric(float refIdx)
        {
            m_RefIdx = refIdx;
            m_R0 = (1.0f - m_RefIdx) / (1.0f + m_RefIdx);
            m_R0 = m_R0 * m_R0;
        }

        public bool Scatter(in Ray inRay, HitRecord rec, ref uint state, out Vector3 attenuation, out Ray scattered)
        {
            Vector3 outwardNormal;
            float niOverNt;
            float reflectProb;
            float cosine;
            if (Vector3.Dot(inRay.Direction, rec.normal) > 0)
            {
                outwardNormal = -rec.normal;
                niOverNt = m_RefIdx;
                cosine = Vector3.Dot(inRay.Direction, rec.normal) / inRay.Direction.Length();
                cosine = MathF.Sqrt(1 - m_RefIdx*m_RefIdx*(1-cosine*cosine));
            }
            else
            {
                outwardNormal = rec.normal;
                niOverNt = 1.0f / m_RefIdx;
                cosine = -Vector3.Dot(inRay.Direction, rec.normal) / inRay.Direction.Length();
            }

            if (Refract(inRay.Direction, outwardNormal, niOverNt, out var refracted))
                reflectProb = Schlick(cosine);
            else
                reflectProb = 1.0f;

            attenuation = Vector3.One;
            if (Rng.Next(ref state) < reflectProb)
            {
                var reflected = Vector3.Reflect(inRay.Direction, rec.normal);
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
