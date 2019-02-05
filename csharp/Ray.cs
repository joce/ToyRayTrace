using System;

namespace ToyRayTrace
{
    public readonly struct Ray
    {
        public Vec3 Origin { get; }
        public Vec3 Direction { get; }

        public Ray(in Vec3 origin, in Vec3 destination)
        {
            Origin = origin;
            Direction = Vec3.Normalize(destination);
        }

        //Origin + Direction * t;
        public Vec3 PointAtParameter(float t) => new Vec3(Origin.x + Direction.x * t, Origin.y + Direction.y * t, Origin.z + Direction.z * t);
    }
}
