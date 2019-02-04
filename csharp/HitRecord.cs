using System;

namespace ToyRayTrace
{
    public struct HitRecord
    {
        public float t;
        public Vec3 p;
        public Vec3 normal;
        public IMaterial material;
    }
}
