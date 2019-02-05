using System;
using System.Collections.Generic;
using System.Linq;

namespace ToyRayTrace
{
    public class HitableList : IHitable
    {
        readonly List<IHitable> m_Hitables;

        public HitableList():
            this(Enumerable.Empty<IHitable>())
        {
        }

        public HitableList(IEnumerable<IHitable> hitables)
        {
            m_Hitables = hitables.ToList();
        }

        public void Add(IHitable hitable) => m_Hitables.Add(hitable);

        public bool Hit(in Ray r, float min, float max, ref HitRecord rec)
        {
            var tempRec = new HitRecord();
            var hitAnything = false;
            var closest = max;
            foreach (var hitable in m_Hitables)
            {
                if (hitable.Hit(r, min, closest, ref tempRec))
                {
                    hitAnything = true;
                    closest = tempRec.t;
                    rec = tempRec;
                }
            }

            return hitAnything;
        }
    }
}
