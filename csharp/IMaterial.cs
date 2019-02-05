using System;

namespace ToyRayTrace
{
    public interface IMaterial
    {
        bool Scatter(in Ray inRay, HitRecord rec, out Vec3 attenuation, out Ray scattered);
    }
}
