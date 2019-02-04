using System;

namespace ToyRayTrace
{
    public class Ray
    {
        public Vec3 Origin { get; }
        public Vec3 Direction { get; }

        public Ray(Vec3 origin, Vec3 destination)
        {
            Origin = origin;
            Direction = Vec3.Normalize(destination);
        }

        public Vec3 PointAtParameter(float t) => Origin + t * Direction;
    }
}
