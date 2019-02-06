using System;
using System.Numerics;

namespace ToyRayTrace
{
    public readonly struct Ray
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public Ray(in Vector3 origin, in Vector3 destination)
        {
            Origin = origin;
            Direction = Vector3.Normalize(destination);
        }

        //Origin + Direction * t;
        public Vector3 PointAtParameter(float t) => new Vector3(Origin.X + Direction.X * t, Origin.Y + Direction.Y * t, Origin.Z + Direction.Z * t);
    }
}
