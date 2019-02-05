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

        public Vec3 PointAtParameter(float t) => Origin + t * Direction;
    }
}
