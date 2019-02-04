using System;

namespace ToyRayTrace
{
    public class Lambertian : IMaterial
    {
        readonly Vec3 m_Albedo;

        public Lambertian(Vec3 albedo)
        {
            m_Albedo = albedo;
        }

        public bool Scatter(Ray inRay, HitRecord rec, out Vec3 attenuation, out Ray scattered)
        {
            var target = rec.p + rec.normal + Rng.NextInUnitSphere();
            scattered = new Ray(rec.p, target - rec.p);
            attenuation = m_Albedo;
            return true;
        }
    }
}
