using System;

namespace ToyRayTrace
{
    public class Lambertian : IMaterial
    {
        readonly Vec3 m_Albedo;

        public Lambertian(in Vec3 albedo)
        {
            m_Albedo = albedo;
        }

        public bool Scatter(in Ray inRay, HitRecord rec, out Vec3 attenuation, out Ray scattered)
        {
            var target = rec.normal + Rng.NextInUnitSphere();
            scattered = new Ray(rec.p, target);
            attenuation = m_Albedo;
            return true;
        }
    }
}
