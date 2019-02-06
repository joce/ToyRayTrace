using System;
using System.Numerics;

namespace ToyRayTrace
{
    public interface IMaterial
    {
        bool Scatter(in Ray inRay, HitRecord rec, ref uint state, out Vector3 attenuation, out Ray scattered);
    }
}
