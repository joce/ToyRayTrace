using System;

namespace ToyRayTrace
{
    public interface IHitable
    {
        bool Hit(Ray r, float min, float max, ref HitRecord rec);
    }
}
