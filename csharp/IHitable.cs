using System;

namespace ToyRayTrace
{
    public interface IHitable
    {
        bool Hit(in Ray r, float min, float max, ref HitRecord rec);
    }
}
