using System;
using System.Numerics;

namespace ToyRayTrace
{
    public class Sphere : IHitable
    {
        readonly Vector3 m_Center;
        readonly float m_Radius;
        readonly IMaterial m_Material;

        public Sphere():
            this(Vector3.Zero, 1f)
        {}

        public Sphere(in Vector3 center, float radius, IMaterial material = null)
        {
            m_Center = center;
            m_Radius = radius;
            m_Material = material;
        }

        public bool Hit(in Ray r, float min, float max, ref HitRecord rec)
        {
            var oc = r.Origin - m_Center;
            var a = Vector3.Dot(r.Direction, r.Direction);
            var b = Vector3.Dot(oc, r.Direction);
            var c = Vector3.Dot(oc, oc) - m_Radius * m_Radius;
            var discriminant = b * b - a * c;
            if (discriminant > 0)
            {
                var sqrt = MathF.Sqrt(discriminant);
                var temp = (-b - sqrt) / a;
                if (temp < max && temp > min)
                {
                    rec.t = temp;
                    rec.p = r.PointAtParameter(rec.t);
//                    rec.normal = (rec.p - m_Center) / m_Radius;
                    rec.normal = new Vector3((rec.p.X - m_Center.X)/m_Radius, (rec.p.Y - m_Center.Y)/m_Radius, (rec.p.Z - m_Center.Z)/m_Radius);
                    rec.material = m_Material;
                    return true;
                }
                temp = (-b + sqrt) / a;
                if (temp < max && temp > min)
                {
                    rec.t = temp;
                    rec.p = r.PointAtParameter(rec.t);
//                    rec.normal = (rec.p - m_Center) / m_Radius;
                    rec.normal = new Vector3((rec.p.X - m_Center.X)/m_Radius, (rec.p.Y - m_Center.Y)/m_Radius, (rec.p.Z - m_Center.Z)/m_Radius);
                    rec.material = m_Material;
                    return true;
                }
            }

            return false;
        }
    }
}
