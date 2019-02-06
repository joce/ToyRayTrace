using System;
using System.Numerics;

namespace ToyRayTrace
{
    public class Lambertian : IMaterial
    {
        readonly Vector3 m_Albedo;

        public Lambertian(in Vector3 albedo)
        {
            m_Albedo = albedo;
        }

        public bool Scatter(in Ray inRay, HitRecord rec, ref uint state, out Vector3 attenuation, out Ray scattered)
        {
            var target = rec.normal + Rng.NextInUnitSphere(ref state);
            scattered = new Ray(rec.p, target);
            attenuation = m_Albedo;
            return true;
        }
    }
}
