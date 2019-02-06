using System;
using System.Numerics;

namespace ToyRayTrace
{
    public struct HitRecord
    {
        public float t;
        public Vector3 p;
        public Vector3 normal;
        public IMaterial material;
    }
}
