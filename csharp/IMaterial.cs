using System;

namespace ToyRayTrace
{
    public interface IMaterial
    {
        bool Scatter(in Ray inRay, HitRecord rec, ref uint state, out Vec3 attenuation, out Ray scattered);
    }
}
