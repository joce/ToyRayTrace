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
            for (int i = 0; i < m_Hitables.Count; i++)
            {
                if (m_Hitables[i].Hit(r, min, closest, ref tempRec))
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
