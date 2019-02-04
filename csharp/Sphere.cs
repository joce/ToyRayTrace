using System;

namespace ToyRayTrace
{
    public class Sphere : IHitable
    {
        Vec3 m_Center;
        float m_Radius;
        readonly IMaterial m_Material;

        public Sphere():
            this(new Vec3(0, 0, 0), 1f)
        {}

        public Sphere(Vec3 center, float radius, IMaterial material = null)
        {
            m_Center = center;
            m_Radius = radius;
            m_Material = material;
        }

        public bool Hit(Ray r, float min, float max, ref HitRecord rec)
        {
            var oc = r.Origin - m_Center;
            var a = Vec3.Dot(r.Direction, r.Direction);
            var b = Vec3.Dot(oc, r.Direction);
            var c = Vec3.Dot(oc, oc) - m_Radius * m_Radius;
            var discriminant = b * b - a * c;
            if (discriminant > 0)
            {
                var sqrt = MathF.Sqrt(discriminant);
                var temp = (-b - sqrt) / a;
                if (temp < max && temp > min)
                {
                    rec.t = temp;
                    rec.p = r.PointAtParameter(rec.t);
                    rec.normal = (rec.p - m_Center) / m_Radius;
                    rec.material = m_Material;
                    return true;
                }
                temp = (-b + sqrt) / a;
                if (temp < max && temp > min)
                {
                    rec.t = temp;
                    rec.p = r.PointAtParameter(rec.t);
                    rec.normal = (rec.p - m_Center) / m_Radius;
                    rec.material = m_Material;
                    return true;
                }
            }

            return false;
        }
    }
}
